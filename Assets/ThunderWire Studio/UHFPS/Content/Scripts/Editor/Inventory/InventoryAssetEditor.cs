using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UHFPS.Scriptable;
using ThunderWire.Editors;

namespace UHFPS.Editors
{
    [CustomEditor(typeof(InventoryAsset))]
    public class InventoryAssetEditor : Editor
    {
        private const int MAX_ITEMS = 15;

        private static InventoryAsset asset;
        private bool seeMore = false;

        private GUIStyle centeredHelpBox
        {
            get => new(EditorStyles.helpBox)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };
        }

        private void OnEnable()
        {
            asset = (InventoryAsset)target;
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            var obj = EditorUtility.InstanceIDToObject(instanceId);
            var asset = obj as InventoryAsset;
            if (asset == null) return false;

            OpenDatabaseEditor();
            return true;
        }

        static void OpenDatabaseEditor()
        {
            EditorWindow invBuilder = EditorWindow.GetWindow<InventoryBuilder>(false, "Inventory Builder", true);

            Vector2 windowSize = new Vector2(1000, 500);
            invBuilder.minSize = windowSize;

            (invBuilder as InventoryBuilder).Show(asset);
        }

        public override void OnInspectorGUI()
        {
            EditorDrawing.DrawInspectorHeader(new GUIContent("Inventory Database"));
            EditorGUILayout.Space();

            serializedObject.Update();
            {
                if(asset.Items.Count > 0)
                {
                    string items, ellipsis;

                    if (!seeMore)
                    {
                        int count = asset.Items.Count - MAX_ITEMS;
                        ellipsis = asset.Items.Count > MAX_ITEMS ? $", (... {count})" : "";
                        items = string.Join(", ", asset.Items.Select(x => x.item.Title).Take(MAX_ITEMS));
                    }
                    else
                    {
                        ellipsis = "";
                        items = string.Join(", ", asset.Items.Select(x => x.item.Title));
                    }

                    using (new EditorDrawing.BorderBoxScope())
                    {
                        GUIContent title = EditorDrawing.IconTextContent("Stored Items", "Prefab On Icon", 14f);
                        EditorGUILayout.LabelField(title, EditorStyles.miniBoldLabel);
                        EditorDrawing.ResetIconSize();
                        EditorGUILayout.Space(2f);

                        EditorGUILayout.LabelField(items + ellipsis, EditorStyles.wordWrappedMiniLabel);

                        if (asset.Items.Count > 15 && !seeMore)
                        {
                            if (GUILayout.Button("See More", EditorStyles.miniButton, GUILayout.Width(70f)))
                            {
                                seeMore = true;
                            }
                        }
                    }
                }
                else
                {
                    Rect labelRect = EditorGUILayout.GetControlRect(GUILayout.Height(25f));
                    GUI.Box(labelRect, "Inventory Database is Empty!", centeredHelpBox);
                }

                EditorGUILayout.Space(2f);
                EditorDrawing.Separator();
                EditorGUILayout.Space(2f);

                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    {
                        if (GUILayout.Button("Open Inventory Builder", GUILayout.Width(180f), GUILayout.Height(25)))
                        {
                            OpenDatabaseEditor();
                        }
                    }
                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndHorizontal();
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}