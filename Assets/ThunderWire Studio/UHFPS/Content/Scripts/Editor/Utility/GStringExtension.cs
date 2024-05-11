using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEditor.Events;
using TMPro;
using UHFPS.Runtime;
using UHFPS.Scriptable;
using ThunderWire.Editors;

namespace UHFPS.Editors
{
    public class GStringExtension : EditorWindow
    {
        const string LASTKEY = "GStringExtension.LastKey";

        private TextMeshProUGUI textMesh;
        private GameLocalizationAsset localizationAsset;

        private WindowProperties propertiesWindow;
        private SerializedObject serializedObject;
        private PropertyCollection properties;

        [MenuItem("CONTEXT/TextMeshProUGUI/GString Localize")]
        static void GStringLocalize(MenuCommand command)
        {
            if (command.context is not TextMeshProUGUI textMesh)
                return;

            EditorWindow window = GetWindow<GStringExtension>(false, "TextMeshPro to GString", true);
            window.minSize = new Vector2(500, 150);
            window.maxSize = new Vector2(500, 150);
            (window as GStringExtension).Show(textMesh);
        }

        public void Show(TextMeshProUGUI textMesh)
        {
            this.textMesh = textMesh;
            localizationAsset = GameLocalization.Instance.LocalizationAsset;

            propertiesWindow = CreateInstance<WindowProperties>();
            serializedObject = new SerializedObject(propertiesWindow);
            properties = EditorDrawing.GetAllProperties(serializedObject);

            if (EditorPrefs.HasKey(LASTKEY))
            {
                string lastKey = EditorPrefs.GetString(LASTKEY);
                string section = lastKey.Split('.')[0];
                string newKey = textMesh.text.ToLower().Replace(" ", ".");

                propertiesWindow.LocalizationKey.GlocText = section + "." + newKey;
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
            }
        }

        private void OnDestroy()
        {
            EditorPrefs.SetString(LASTKEY, propertiesWindow.LocalizationKey.GlocText);

            properties = null;
            serializedObject = null;
            DestroyImmediate(propertiesWindow);
        }

        private void OnGUI()
        {
            Rect rect = position;
            rect.xMin += 5f;
            rect.xMax -= 5f;
            rect.yMin += 5f;
            rect.yMax -= 5f;
            rect.x = 5;
            rect.y = 5;

            GUILayout.BeginArea(rect);
            {
                EditorGUILayout.HelpBox("With this tool, you can easily Assign or Generate a GString Localization Key that will be automatically linked to the UI Text Component.", MessageType.Info);
                EditorGUILayout.Space();

                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    localizationAsset = (GameLocalizationAsset)EditorGUILayout.ObjectField(new GUIContent("GameLocalization Asset"), localizationAsset, typeof(GameLocalizationAsset), false);
                    properties.Draw("LocalizationKey");
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();

                string gLocKey = propertiesWindow.LocalizationKey.GlocText;
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Assign Key", GUILayout.Height(30f), GUILayout.Width(240)))
                    {
                        AssignKey(false);
                        Debug.Log($"The localization key '{gLocKey}' has been assigned and linked to the Text Component.");
                    }

                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Create & Assign Key", GUILayout.Height(30f), GUILayout.Width(240)))
                    {
                        AssignKey(true);
                        Debug.Log($"The localization key '{gLocKey}' has been added to the localization asset and linked to the Text Component.");
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.EndArea();
        }

        private async void AssignKey(bool create)
        {
            string gLocKey = propertiesWindow.LocalizationKey.GlocText;

            if (string.IsNullOrEmpty(gLocKey))
                return;

            if (!textMesh.gameObject.TryGetComponent(out GLocText gLoc))
                gLoc = textMesh.gameObject.AddComponent<GLocText>();

            await WaitForGLocNull(gLoc);

            gLoc.GlocKey.GlocText = gLocKey;

            if (create)
            {
                string text = textMesh.text;
                localizationAsset.AddSectionKey(gLocKey, text);
            }

            var setStringMethod = textMesh.GetType().GetProperty("text").GetSetMethod();
            var methodDelegate = Delegate.CreateDelegate(typeof(UnityAction<string>), textMesh, setStringMethod) as UnityAction<string>;
            UnityEventTools.AddPersistentListener(gLoc.OnUpdateText, methodDelegate);
            gLoc.OnUpdateText.SetPersistentListenerState(0, UnityEventCallState.RuntimeOnly);
        }

        private async Task WaitForGLocNull(GLocText gloc)
        {
            while (gloc.GlocKey == null)
            {
                await Task.Yield();
            }
        }

        public sealed class WindowProperties : ScriptableObject
        {
            public GString LocalizationKey;
        }
    }
}