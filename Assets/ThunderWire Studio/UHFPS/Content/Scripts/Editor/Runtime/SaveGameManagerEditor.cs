using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UHFPS.Runtime;
using ThunderWire.Editors;

namespace UHFPS.Editors
{
    [CustomEditor(typeof(SaveGameManager))]
    public class SaveGameManagerEditor : Editor
    {
        private SaveGameManager manager;
        private string saveFolderName;
        private bool debugExpanded;

        private void OnEnable()
        {
            manager = (SaveGameManager)target;
            saveFolderName = "Save";
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorDrawing.DrawInspectorHeader(new GUIContent("Save Game Manager"), target);
            EditorGUILayout.Space();
            DrawPropertiesExcluding(serializedObject, "m_Script", "worldSaveables", "runtimeSaveables");

            GUIStyle headerStyle = new(EditorStyles.boldLabel)
            {
                fontSize = 11,
                alignment = TextAnchor.MiddleLeft,
                padding = new(5, 0, 0, 0)
            };

            headerStyle.normal.textColor = Color.white;

            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                Rect searcherHeader = EditorGUILayout.GetControlRect(false, 25f);
                ColorUtility.TryParseHtmlString("#272727", out Color color);

                EditorGUI.DrawRect(searcherHeader, color);
                EditorGUI.LabelField(searcherHeader, "Saveables Searcher".ToUpper(), headerStyle);

                using (new EditorDrawing.IconSizeScope(15))
                {
                    EditorUtils.TrIconText($"World Saveables: {manager.worldSaveables.Count}", MessageType.Info, EditorStyles.miniBoldLabel);
                    EditorUtils.TrIconText($"Runtime Saveables: {manager.runtimeSaveables.Count}", MessageType.Info, EditorStyles.miniBoldLabel);
                }
                EditorGUILayout.Space();

                using (new EditorDrawing.BackgroundColorScope("#F7E987"))
                {
                    if (GUILayout.Button("Find Saveables", GUILayout.Height(25)))
                    {
                        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                        stopwatch.Start();

                        MonoBehaviour[] monos = FindObjectsOfType<MonoBehaviour>(true);
                        var saveables = from mono in monos
                                        let type = mono.GetType()
                                        where typeof(ISaveable).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract
                                        let token = $"{type.Name}{SaveGameManager.TOKEN_SEPARATOR}{GUID.Generate()}"
                                        select new SaveGameManager.SaveablePair(token, mono);

                        manager.worldSaveables = saveables.ToList();
                        stopwatch.Stop();

                        EditorUtility.SetDirty(target);
                        Debug.Log($"<color=yellow>[Saveables Searcher]</color> Found {saveables.Count()} world saveables in {stopwatch.ElapsedMilliseconds}ms. <color=red>SAVE YOUR SCENE!</color>");
                    }
                }

                Rect resetButtonRect = searcherHeader;
                resetButtonRect.xMin = resetButtonRect.xMax - EditorGUIUtility.singleLineHeight;
                resetButtonRect.height = EditorGUIUtility.singleLineHeight;
                resetButtonRect.y += 4f;
                resetButtonRect.x -= 5f;

                GUIContent resetButtonIcon = EditorGUIUtility.TrIconContent("Refresh", "Reset Saveables");
                if (GUI.Button(resetButtonRect, resetButtonIcon, EditorStyles.iconButton))
                {
                    manager.worldSaveables.Clear();
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            if(EditorDrawing.BeginFoldoutBorderLayout(new GUIContent("Functions Debug (Runtime Only)"), ref debugExpanded))
            {
                using (new EditorGUI.DisabledGroupScope(!Application.isPlaying))
                {
                    Rect saveGameRect = EditorGUILayout.GetControlRect();
                    Rect saveGameBtn = EditorGUI.PrefixLabel(saveGameRect, new GUIContent("Save Game"));
                    if (GUI.Button(saveGameBtn, new GUIContent("Save")))
                    {
                        SaveGameManager.SaveGame(false);
                    }

                    Rect loadGameRect = EditorGUILayout.GetControlRect();
                    Rect loadGameBtn = EditorGUI.PrefixLabel(loadGameRect, new GUIContent("Load Game"));

                    Rect loadGameText = loadGameBtn;
                    loadGameText.xMax *= 0.8f;
                    loadGameBtn.xMin = loadGameText.xMax + 2f;

                    saveFolderName = EditorGUI.TextField(loadGameText, saveFolderName);
                    if (GUI.Button(loadGameBtn, new GUIContent("Load")))
                    {
                        if (!string.IsNullOrEmpty(saveFolderName))
                        {
                            SaveGameManager.GameLoadType = SaveGameManager.LoadType.LoadGameState;
                            SaveGameManager.LoadFolderName = saveFolderName;

                            string sceneName = SceneManager.GetActiveScene().name;
                            SaveGameManager.LoadSceneName = sceneName;
                            SceneManager.LoadScene(SaveGameManager.LMS);
                        }
                    }
                }

                EditorDrawing.EndBorderHeaderLayout();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private Texture2D MakeBackgroundTexture(int width, int height, Color color)
        {
            Color[] pixels = new Color[width * height];

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }

            Texture2D backgroundTexture = new Texture2D(width, height);

            backgroundTexture.SetPixels(pixels);
            backgroundTexture.Apply();

            return backgroundTexture;
        }
    }
}