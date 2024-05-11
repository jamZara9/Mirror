using System;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Reactive.Disposables;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UHFPS.Tools;
using UHFPS.Input;
using UHFPS.Scriptable;
using ThunderWire.Attributes;

namespace UHFPS.Runtime
{
    [Docs("https://docs.twgamesdev.com/uhfps/guides/options-manager")]
    public partial class OptionsManager : Singleton<OptionsManager>
    {
        public const char NAME_SEPARATOR = '.';
        public const string EXTENSION = ".json";

        public enum OptionTypeEnum { Custom, Monitor, Resolution, Fullscreen, FrameRate, VSync, RenderScale, FSRSharpness, Antialiasing, Anisotropic, TextureQuality, ShadowDistance, GlobalVolume }
        public enum OptionValueEnum { Boolean, Integer, Float, String }

        private static readonly int[] Framerates = { 30, 60, 120, -1 };
        private static readonly int[] Antialiasing = { 1, 2, 4, 8 };
        private static readonly int[] ShadowDistances = { 0, 25, 40, 55, 70, 85, 100 };

        [Serializable]
        public struct OptionObject
        {
            public string Name;
            public OptionBehaviour Option;
            public OptionTypeEnum OptionType;
            public OptionValueEnum OptionValue;
            public string DefaultValue;
        }

        [Serializable]
        public struct OptionSection
        {
            public string Section;
            public List<OptionObject> Options;
        }

        public List<OptionSection> Options = new();
        public bool ApplyAndSaveInputs = true;
        public bool ShowDebug = true;

        private UniversalRenderPipelineAsset URPAsset => (UniversalRenderPipelineAsset)GraphicsSettings.renderPipelineAsset;

        private SerializationAsset SerializationAsset
            => SerializationUtillity.SerializationAsset;

        private string OptionsFilename => SerializationUtillity.SerializationAsset.OptionsFilename + EXTENSION;

        private string OptionsPath
        {
            get
            {
                string configPath = SerializationAsset.GetConfigPath();
                if (!Directory.Exists(configPath))
                    Directory.CreateDirectory(configPath);

                return configPath + "/" + OptionsFilename;
            }
        }

        public static bool IsLoaded { get; private set; }

        private readonly Dictionary<string, BehaviorSubject<object>> optionSubjects = new();
        private static Dictionary<string, JValue> serializableData = new();

        private readonly CompositeDisposable disposables = new();
        private readonly List<DisplayInfo> displayInfos = new();

        private List<Resolution> resolutions;
        private readonly ObservableValue<DisplayInfo> currentDisplay = new();
        private readonly ObservableValue<FullScreenMode> currentFullscreen = new();
        private readonly ObservableValue<Resolution> currentResolution = new();

        [ContextMenu("Reset Loaded Options")]
        private void ResetOptions()
        {
            IsLoaded = false;
            serializableData.Clear();
        }

        private void Awake()
        {
            foreach (var section in Options)
            {
                foreach (var option in section.Options)
                {
                    string name = option.Name.ToLower();
                    string _val = string.IsNullOrEmpty(option.DefaultValue) ? "0" : option.DefaultValue;
                    optionSubjects[name] = new BehaviorSubject<object>(option.OptionValue switch
                    {
                        OptionValueEnum.Boolean => int.Parse(_val) == 1,
                        OptionValueEnum.Integer => int.Parse(_val),
                        OptionValueEnum.Float => float.Parse(_val),
                        _ => _val,
                    });
                }
            }
        }

        private void Start()
        {
            SetOptionDatas();
            LoadOptions();
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }

        private void SetOptionDatas()
        {
            var monitor = GetOption(OptionTypeEnum.Monitor);
            var resolution = GetOption(OptionTypeEnum.Resolution);

            Screen.GetDisplayLayout(displayInfos);
            string[] displays = displayInfos.Select(x => x.name).ToArray();

            resolutions = Screen.resolutions.ToList();
            string[] _resolutions = resolutions.Select(x => $"{x.width}x{x.height}@{x.refreshRateRatio}").ToArray();

            monitor?.Option.SetOptionData(displays);
            resolution?.Option.SetOptionData(_resolutions);
        }

        public static void ObserveOption(string name, Action<object> onChange)
        {
            if(Instance.optionSubjects.TryGetValue(name, out var subject))
                subject.Subscribe(onChange).AddTo(Instance.disposables);
        }

        public OptionObject? GetOption(OptionTypeEnum optionType)
        {
            foreach (var section in Options)
            {
                foreach (var option in section.Options)
                {
                    if (option.OptionType == optionType)
                        return option;
                }
            }

            return null;
        }

        public OptionObject? GetOption(string optionName)
        {
            string[] path = optionName.Split(NAME_SEPARATOR);
            foreach (var section in Options)
            {
                if (section.Section != path[0])
                    continue;

                foreach (var option in section.Options)
                {
                    if (option.Name == path[1])
                        return option;
                }
            }

            return null;
        }

        public async void ApplyOptions()
        {
            foreach (var section in Options)
            {
                foreach (var option in section.Options)
                {
                    ApplyOptionsRealtime(option);
                }
            }

            ApplyResolution();
            await SerializeOptions();

            if(ApplyAndSaveInputs) InputManager.ApplyInputRebindOverrides();
            if(ShowDebug) Debug.Log($"[OptionsManager] The option values have been saved to '{OptionsFilename}'.");
        }

        public void DiscardChanges()
        {
            bool anyDiscard = false;
            foreach (var section in Options)
            {
                foreach (var option in section.Options)
                {
                    if (!option.Option.IsChanged)
                        continue;

                    LoadOptions(option, false);
                    anyDiscard = true;
                }
            }

            if(ApplyAndSaveInputs) InputManager.ResetInputsToDefaults();
            if(ShowDebug && anyDiscard) Debug.Log("[OptionsManager] Options Discarded");
        }

        private async void LoadOptions()
        {
            bool fromFile = IsLoaded || File.Exists(OptionsPath);

            if (fromFile && !IsLoaded)
            {
                await DeserializeOptions();
                if (ShowDebug) Debug.Log("[OptionsManager] The options have been loaded.");
            }

            foreach (var section in Options)
            {
                foreach (var option in section.Options)
                {
                    LoadOptions(option, fromFile);
                }
            }

            if(!IsLoaded) ApplyResolution();
            IsLoaded = true;
        }

        private void ApplyResolution()
        {
            int screenWidth = currentResolution.Value.width;
            int screenHeight = currentResolution.Value.height;
            var fullscreen = currentFullscreen.Value;

            if (currentResolution.IsChanged && currentFullscreen.IsChanged)
            {
                Screen.SetResolution(screenWidth, screenHeight, fullscreen);
            }
            else if (currentResolution.IsChanged)
            {
                Screen.SetResolution(screenWidth, screenHeight, Screen.fullScreenMode);
            }
            else if (currentFullscreen.IsChanged)
            {
                Screen.fullScreenMode = fullscreen;
            }
        }

        private void ApplyOptionsRealtime(OptionObject option)
        {
            OptionTypeEnum optionType = option.OptionType;
            string name = option.Name.ToLower();

            bool isChanged = option.Option.IsChanged;
            object obj = option.Option.GetOptionValue();

            Dictionary<OptionTypeEnum, Action> options = new()
            {
                { OptionTypeEnum.Custom, () => ApplyCustomOption(option, name, obj) },
                { OptionTypeEnum.Monitor, () => ApplyMonitorOption(name, (int)obj, isChanged) },
                { OptionTypeEnum.Resolution, () => ApplyResolutionOption((int)obj, isChanged) },
                { OptionTypeEnum.Fullscreen, () => ApplyFullscreenOption((int)obj, isChanged) },
                { OptionTypeEnum.FrameRate, () => ApplyFramerateOption(name, (int)obj, isChanged) },
                { OptionTypeEnum.VSync, () => ApplyVSyncOption(name, (int)obj, isChanged) },
                { OptionTypeEnum.RenderScale, () => ApplyRenderScaleOption(name, (float)obj, isChanged) },
                { OptionTypeEnum.FSRSharpness, () => ApplyFSRSharpnessOption(name, (float)obj, isChanged) },
                { OptionTypeEnum.Antialiasing, () => ApplyAntialiasingOption(name, (int)obj, isChanged) },
                { OptionTypeEnum.Anisotropic, () => ApplyAnisotropicOption(name, (int)obj, isChanged) },
                { OptionTypeEnum.TextureQuality, () => ApplyTextureQualityOption(name, (int)obj, isChanged) },
                { OptionTypeEnum.ShadowDistance, () => ApplyShadowDistanceOption(name, (int)obj, isChanged) },
                { OptionTypeEnum.GlobalVolume, () => ApplyGlobalVolumeOption(name, (float)obj, isChanged) }
            };

            options[optionType].Invoke();
            option.Option.IsChanged = false;
        }

        private void ApplyCustomOption(OptionObject option, string name, object obj)
        {
            object convertedValue = option.OptionValue switch
            {
                OptionValueEnum.Boolean => Convert.ToBoolean(obj),
                OptionValueEnum.Integer => Convert.ToInt32(obj),
                OptionValueEnum.Float => Convert.ToSingle(obj),
                OptionValueEnum.String => obj.ToString(),
                _ => obj.ToString(),
            };

            if (option.Option.IsChanged && optionSubjects.TryGetValue(name, out var subject))
                subject.OnNext(convertedValue);

            serializableData[name] = new(convertedValue);
        }

        private void LoadOptions(OptionObject option, bool fromFile)
        {
            var behaviour = option.Option;
            var optionType = option.OptionType;
            string name = option.Name.ToLower();

            Dictionary<OptionTypeEnum, Action> options = new()
            {
                { OptionTypeEnum.Custom, () => LoadCustomOption(name, fromFile, behaviour, option) },
                { OptionTypeEnum.Monitor, () => LoadMonitorOption(name, fromFile, behaviour) },
                { OptionTypeEnum.Resolution, () => LoadResoltionOption(name, fromFile, behaviour) },
                { OptionTypeEnum.Fullscreen, () => LoadFullscreenOption(name, fromFile, behaviour) },
                { OptionTypeEnum.FrameRate, () => LoadFramerateOption(name, fromFile, behaviour) },
                { OptionTypeEnum.VSync, () => LoadVSyncOption(name, fromFile, behaviour) },
                { OptionTypeEnum.RenderScale, () => LoadRenderScaleOption(name, fromFile, behaviour) },
                { OptionTypeEnum.FSRSharpness, () => LoadFSRSharpnessOption(name, fromFile, behaviour) },
                { OptionTypeEnum.Antialiasing, () => LoadAntialiasingOption(name, fromFile, behaviour) },
                { OptionTypeEnum.Anisotropic, () => LoadAnisotropicOption(name, fromFile, behaviour) },
                { OptionTypeEnum.TextureQuality, () => LoadTextureQualityOption(name, fromFile, behaviour) },
                { OptionTypeEnum.ShadowDistance, () => LoadShadowDistanceOption(name, fromFile, behaviour) },
                { OptionTypeEnum.GlobalVolume, () => LoadGlobalVolumeOption(name, fromFile, behaviour) }
            };

            options[optionType].Invoke();
        }

        private void LoadCustomOption(string name, bool fromFile, OptionBehaviour behaviour, OptionObject option)
        {
            object value = string.IsNullOrEmpty(option.DefaultValue) ? "0" : option.DefaultValue;
            object optionValue, subjectValue;

            if (fromFile && serializableData.TryGetValue(name, out JValue jValue))
            {
                optionValue = option.OptionValue switch
                {
                    OptionValueEnum.Boolean => jValue.ToObject<bool>() ? 1 : 0,
                    OptionValueEnum.Integer => jValue.ToObject<int>(),
                    OptionValueEnum.Float => jValue.ToObject<float>(),
                    _ => jValue.ToString(),
                };

                subjectValue = option.OptionValue switch
                {
                    OptionValueEnum.Boolean => jValue.ToObject<bool>(),
                    OptionValueEnum.Integer => jValue.ToObject<int>(),
                    OptionValueEnum.Float => jValue.ToObject<float>(),
                    _ => jValue.ToString(),
                };
            }
            else
            {
                optionValue = option.OptionValue switch
                {
                    OptionValueEnum.Boolean => Convert.ToInt32(value),
                    OptionValueEnum.Integer => Convert.ToInt32(value),
                    OptionValueEnum.Float => Convert.ToSingle(value),
                    _ => value.ToString(),
                };

                subjectValue = option.OptionValue switch
                {
                    OptionValueEnum.Boolean => Convert.ToInt32(value) == 1,
                    OptionValueEnum.Integer => Convert.ToInt32(value),
                    OptionValueEnum.Float => Convert.ToSingle(value),
                    _ => value.ToString(),
                };
            }

            if (optionSubjects.TryGetValue(name, out var subject))
                subject.OnNext(subjectValue);

            behaviour.SetOptionValue(optionValue);
        }

        private async Task SerializeOptions()
        {
            string json = JsonConvert.SerializeObject(serializableData, Formatting.Indented, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            await File.WriteAllTextAsync(OptionsPath, json);
        }

        private async Task DeserializeOptions()
        {
            string json = await File.ReadAllTextAsync(OptionsPath);
            serializableData = JsonConvert.DeserializeObject<Dictionary<string, JValue>>(json);
        }
    }
}