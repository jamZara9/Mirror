using System;
using System.Linq;
using UnityEngine;
using UHFPS.Tools;
using Newtonsoft.Json.Linq;

namespace UHFPS.Runtime
{
    public partial class OptionsManager
    {
         // 0 - First, N - Last
        private void LoadMonitorOption(string name, bool fromFile, OptionBehaviour behaviour)
        {
            if (fromFile && CheckOption(name, JTokenType.Integer, out int value))
            {
                if (value < Display.displays.Length)
                {
                    currentDisplay.SilentValue = displayInfos[value];
                    Display.displays[value].Activate();
                    behaviour.SetOptionValue(value);
                    return;
                }
            }

            currentDisplay.SilentValue = Screen.mainWindowDisplayInfo;
            int display = displayInfos.IndexOf(currentDisplay.Value);
            behaviour.SetOptionValue(display);
        }

        // 0 - Min Resolution, N - Max Resolution
        private void LoadResoltionOption(string name, bool fromFile, OptionBehaviour behaviour)
        {
            if (fromFile)
            {
                bool val1 = CheckOption("screen_width", JTokenType.Integer, out int width);
                bool val2 = CheckOption("screen_height", JTokenType.Integer, out int height);

                if (val1 && val2)
                {
                    int index = resolutions.FindIndex(x => x.width == width && x.height == height);
                    if (index <= -1) index = resolutions.Count - 1;

                    currentResolution.SilentValue = Screen.currentResolution;
                    currentResolution.Value = resolutions[index];
                    behaviour.SetOptionValue(index);
                    return;
                }
            }

            currentResolution.SilentValue = Screen.currentResolution;
            int value = resolutions.IndexOf(currentResolution.Value);
            behaviour.SetOptionValue(value);
        }

        // 0 - Windowed, 1 - Fullscreen
        private void LoadFullscreenOption(string name, bool fromFile, OptionBehaviour behaviour)
        {
            if (fromFile && CheckOption("screen_fullscreen", JTokenType.Boolean, out bool fullscreen))
            {
                currentFullscreen.SilentValue = Screen.fullScreenMode;
                currentFullscreen.Value = fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
                behaviour.SetOptionValue(fullscreen ? 1 : 0);
                return;
            }

            currentFullscreen.Value = Screen.fullScreenMode;
            int value = currentFullscreen.Value == FullScreenMode.FullScreenWindow ? 1 : 0;
            behaviour.SetOptionValue(value);
        }

        // 0 - 30FPS, 1 - 60FPS, 2 - 120FPS, 3 - Variable
        private void LoadFramerateOption(string name, bool fromFile, OptionBehaviour behaviour)
        {
            if (fromFile && CheckOption(name, JTokenType.Integer, out int value))
            {
                if (Framerates.Any(x => value == x))
                {
                    Application.targetFrameRate = value;
                    value = Array.IndexOf(Framerates, value);
                    behaviour.SetOptionValue(value);
                    return;
                }
            }

            int framerate = Application.targetFrameRate;
            int fIndex = Array.IndexOf(Framerates, framerate);
            behaviour.SetOptionValue(fIndex);
        }

        // 0 - Don't Sync, 1 = Every V Blank
        private void LoadVSyncOption(string name, bool fromFile, OptionBehaviour behaviour)
        {           
            if (fromFile && CheckOption(name, JTokenType.Integer, out int value))
            {
                value = Mathf.Clamp(value, 0, 1);
                QualitySettings.vSyncCount = value;
                behaviour.SetOptionValue(value);
                return;
            }

            int vsync = QualitySettings.vSyncCount;
            behaviour.SetOptionValue(vsync);
        }

        // 0.1 - Min Resolution, 2 - Max Resolution
        private void LoadRenderScaleOption(string name, bool fromFile, OptionBehaviour behaviour)
        {
            if (fromFile && CheckOption(name, JTokenType.Float, out float value))
            {
                value = Mathf.Clamp(value, 0.1f, 2f);
                URPAsset.renderScale = value;
                behaviour.SetOptionValue(value);
                return;
            }

            float renderScale = URPAsset.renderScale;
            behaviour.SetOptionValue(renderScale);
        }

        // 0.1 - Min Resolution, 2 - Max Resolution
        private void LoadFSRSharpnessOption(string name, bool fromFile, OptionBehaviour behaviour)
        {
            if (fromFile && CheckOption(name, JTokenType.Float, out float value))
            {
                value = Mathf.Clamp01(value);
                URPAsset.fsrSharpness = value;
                behaviour.SetOptionValue(value);
                return;
            }

            float fsrSharpness = URPAsset.fsrSharpness;
            behaviour.SetOptionValue(fsrSharpness);
        }

        // 0 - Disabled, 1 - 2x, 2 - 4x, 3 - 8x
        private void LoadAntialiasingOption(string name, bool fromFile, OptionBehaviour behaviour)
        {
            if (fromFile && CheckOption(name, JTokenType.Integer, out int value))
            {
                value = Mathf.Clamp(value, 0, 3);
                URPAsset.msaaSampleCount = Antialiasing[value];
                behaviour.SetOptionValue(value);
                return;
            }

            int antialiasing = URPAsset.msaaSampleCount;
            antialiasing = Array.IndexOf(Antialiasing, antialiasing);
            behaviour.SetOptionValue(antialiasing);
        }

        // 0 - Disable, 1 - Enable, 2 - Force Enable
        private void LoadAnisotropicOption(string name, bool fromFile, OptionBehaviour behaviour)
        {
            if (fromFile && CheckOption(name, JTokenType.Integer, out int value))
            {
                value = Mathf.Clamp(value, 0, 2);
                QualitySettings.anisotropicFiltering = (AnisotropicFiltering)value;
                behaviour.SetOptionValue(value);
                return;
            }

            int anisotropic = (int)QualitySettings.anisotropicFiltering;
            behaviour.SetOptionValue(anisotropic);
        }

        // 0 - Eighth Size, 1 - Quarter Size, 2 - Half Size, 3 - Normal
        private void LoadTextureQualityOption(string name, bool fromFile, OptionBehaviour behaviour)
        {
            if (fromFile && CheckOption(name, JTokenType.Integer, out int value))
            {
                value = Mathf.Clamp(value, 0, 3);
                QualitySettings.globalTextureMipmapLimit = 3 - value;
                behaviour.SetOptionValue(value);
                return;
            }

            int texQuality = QualitySettings.globalTextureMipmapLimit;
            texQuality = 3 - texQuality;
            behaviour.SetOptionValue(texQuality);
        }

        // 0 - 0m (Disabled), 1 - 25m (Very Low), 2 - 40m (Low), 3 - 55m (Medium), 4 - 70m (High), 5 - 85m (Very High), 6 - 100m (Max)
        private void LoadShadowDistanceOption(string name, bool fromFile, OptionBehaviour behaviour)
        {
            if (fromFile && CheckOption(name, JTokenType.Integer, out int value))
            {
                value = Mathf.Clamp(value, 0, 6);
                URPAsset.shadowDistance = ShadowDistances[value];
                behaviour.SetOptionValue(value);
                return;
            }

            float shadowDistance = URPAsset.shadowDistance;
            int distance = ShadowDistances.ClosestIndex((int)shadowDistance);
            behaviour.SetOptionValue(distance);
        }

        // 0 - Min Volume, 1 - Max Volume
        private void LoadGlobalVolumeOption(string name, bool fromFile, OptionBehaviour behaviour)
        {
            if (fromFile && CheckOption(name, JTokenType.Float, out float value))
            {
                value = Mathf.Clamp01(value);
                AudioListener.volume = value;
                behaviour.SetOptionValue(value);
                return;
            }

            float globalVolume = AudioListener.volume;
            behaviour.SetOptionValue(globalVolume);
        }

        private bool CheckOption<T>(string name, JTokenType type, out T value) where T : struct
        {
            if (serializableData.TryGetValue(name, out JValue jValue) && jValue.Type == type)
            {
                value = jValue.ToObject<T>();
                return true;
            }

            value = default;
            return false;
        }
    }
}