using UnityEngine;
using UnityEditor;
using UHFPS.Runtime;
using ThunderWire.Editors;
using static UHFPS.Runtime.OptionsManager;

namespace UHFPS.Editors
{
    [CustomEditor(typeof(OptionsManager))]
    public class OptionsManagerEditor : InspectorEditor<OptionsManager>
    {
        private Vector2 defaultIconSize;

        public override void OnEnable()
        {
            base.OnEnable();
            defaultIconSize = EditorGUIUtility.GetIconSize();
        }

        public override void OnInspectorGUI()
        {
            EditorDrawing.DrawInspectorHeader(new GUIContent("Options Manager"), Target);
            EditorGUILayout.Space();

            serializedObject.Update();
            {
                SerializedProperty options = Properties["Options"];
                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    Rect labelRect = EditorGUILayout.GetControlRect();
                    EditorGUI.LabelField(labelRect, "Options List", EditorStyles.boldLabel);

                    Rect countLabelRect = labelRect;
                    countLabelRect.xMin = countLabelRect.xMax - 25f;

                    GUI.enabled = false;
                    EditorGUI.IntField(countLabelRect, options.arraySize);
                    GUI.enabled = true;

                    EditorGUILayout.Space(2f);

                    if (options.arraySize <= 0)
                    {
                        EditorGUILayout.HelpBox("There are currently no option sections. To create a new section, click the 'Add Section' button.", MessageType.Info);
                    }

                    for (int i = 0; i < options.arraySize; i++)
                    {
                        SerializedProperty section = options.GetArrayElementAtIndex(i);
                        SerializedProperty sectionName = section.FindPropertyRelative("Section");
                        SerializedProperty sectionOptions = section.FindPropertyRelative("Options");

                        string name = sectionName.stringValue;
                        GUIContent header = EditorGUIUtility.TrTextContentWithIcon($" {name} (Section)", "Profiler.UIDetails");

                        EditorGUIUtility.SetIconSize(new Vector2(14, 14));
                        if (EditorDrawing.BeginFoldoutBorderLayout(section, header, out Rect foldoutRect, roundedBox: false))
                        {
                            EditorGUILayout.PropertyField(sectionName, new GUIContent("Section Name"));
                            EditorGUILayout.Space(3f);

                            Rect optionsLabel = EditorGUILayout.GetControlRect();
                            EditorGUI.LabelField(optionsLabel, new GUIContent("Options"), EditorStyles.boldLabel);

                            Rect addOptionRect = optionsLabel;
                            addOptionRect.xMin = addOptionRect.xMax - EditorGUIUtility.singleLineHeight;

                            if (GUI.Button(addOptionRect, EditorUtils.Styles.PlusIcon))
                            {
                                Target.Options[i].Options.Add(new()
                                {
                                    Name = "New Option"
                                });
                            }

                            if (sectionOptions != null && sectionOptions.arraySize > 0)
                            {
                                EditorGUILayout.Space(1f);
                                for (int j = 0; j < sectionOptions.arraySize; j++)
                                {
                                    SerializedProperty optionProperty = sectionOptions.GetArrayElementAtIndex(j);
                                    SerializedProperty optionName = optionProperty.FindPropertyRelative("Name");

                                    string _optionName = optionName.stringValue;
                                    GUIContent optionHeader = EditorGUIUtility.TrTextContentWithIcon($" {_optionName} (Option)", "Settings");

                                    if (EditorDrawing.BeginFoldoutBorderLayout(optionProperty, optionHeader, out Rect optionFoldoutRect))
                                    {
                                        SerializedProperty _option = optionProperty.FindPropertyRelative("Option");
                                        SerializedProperty _optionType = optionProperty.FindPropertyRelative("OptionType");
                                        SerializedProperty _optionValue = optionProperty.FindPropertyRelative("OptionValue");
                                        SerializedProperty _defaultValue = optionProperty.FindPropertyRelative("DefaultValue");

                                        EditorGUILayout.PropertyField(optionName);
                                        EditorGUILayout.PropertyField(_option);
                                        EditorGUILayout.PropertyField(_optionType);

                                        OptionTypeEnum optionType = (OptionTypeEnum)_optionType.enumValueIndex;
                                        if (optionType == OptionTypeEnum.Custom)
                                        {
                                            OptionValueEnum optionValue = (OptionValueEnum)_optionValue.enumValueIndex;

                                            EditorGUI.BeginChangeCheck();
                                            EditorGUILayout.PropertyField(_optionValue);
                                            if (EditorGUI.EndChangeCheck())
                                            {
                                                optionValue = (OptionValueEnum)_optionValue.enumValueIndex;
                                                _defaultValue.stringValue = optionValue switch
                                                {
                                                    OptionValueEnum.String => "",
                                                    _ => "0"
                                                };
                                            }

                                            Rect optionValueRect = EditorGUILayout.GetControlRect();
                                            if (optionValue == OptionValueEnum.Boolean)
                                            {
                                                if (string.IsNullOrEmpty(_defaultValue.stringValue))
                                                    _defaultValue.stringValue = "0";

                                                bool value = int.Parse(_defaultValue.stringValue) == 1;
                                                bool toggle = EditorGUI.Toggle(optionValueRect, _defaultValue.displayName, value);
                                                _defaultValue.stringValue = (toggle ? 1 : 0).ToString();
                                            }
                                            else if (optionValue == OptionValueEnum.Integer)
                                            {
                                                if (string.IsNullOrEmpty(_defaultValue.stringValue))
                                                    _defaultValue.stringValue = "0";

                                                int value = int.Parse(_defaultValue.stringValue);
                                                _defaultValue.stringValue = EditorGUI.IntField(optionValueRect, _defaultValue.displayName, value).ToString();
                                            }
                                            else if (optionValue == OptionValueEnum.Float)
                                            {
                                                if (string.IsNullOrEmpty(_defaultValue.stringValue))
                                                    _defaultValue.stringValue = "0";

                                                float value = float.Parse(_defaultValue.stringValue);
                                                _defaultValue.stringValue = EditorGUI.FloatField(optionValueRect, _defaultValue.displayName, value).ToString();
                                            }
                                            else if (optionValue == OptionValueEnum.String)
                                            {
                                                _defaultValue.stringValue = EditorGUI.TextField(optionValueRect, _defaultValue.displayName, _defaultValue.stringValue);
                                            }
                                        }

                                        using(new EditorGUI.DisabledGroupScope(true))
                                        {
                                            string sectionPath = name.ToLower();
                                            string optionPath = _optionName.ToLower();
                                            string fullOptionPath = $"{sectionPath}.{optionPath}";
                                            EditorGUILayout.TextField("Option Path", fullOptionPath);
                                        }

                                        EditorDrawing.EndBorderHeaderLayout();
                                    }

                                    Rect optionRemoveButton = optionFoldoutRect;
                                    optionRemoveButton.xMin = optionRemoveButton.xMax - EditorGUIUtility.singleLineHeight;
                                    optionRemoveButton.y += 3f;
                                    if (GUI.Button(optionRemoveButton, EditorUtils.Styles.MinusIcon, EditorStyles.iconButton))
                                    {
                                        sectionOptions.DeleteArrayElementAtIndex(j);
                                    }
                                }
                            }
                            else
                            {
                                EditorGUIUtility.SetIconSize(defaultIconSize);
                                EditorGUILayout.HelpBox("There are currently no options. To create a new option, click the 'plus (+)' button.", MessageType.Info);
                            }

                            EditorDrawing.EndBorderHeaderLayout();
                        }

                        Rect removeButton = foldoutRect;
                        removeButton.xMin = removeButton.xMax - EditorGUIUtility.singleLineHeight;
                        removeButton.y += 3f;
                        if (GUI.Button(removeButton, EditorUtils.Styles.MinusIcon, EditorStyles.iconButton))
                        {
                            options.DeleteArrayElementAtIndex(i);
                        }

                        if (i + 1 < options.arraySize) EditorGUILayout.Space(1f);
                    }

                    EditorGUIUtility.SetIconSize(defaultIconSize);

                    EditorGUILayout.Space(2f);
                    EditorDrawing.Separator();
                    EditorGUILayout.Space(2f);

                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        {
                            Rect addSectionRect = EditorGUILayout.GetControlRect(GUILayout.Width(100f));
                            if (GUI.Button(addSectionRect, "Add Section"))
                            {
                                Target.Options.Add(new OptionSection()
                                {
                                     Section = "New Section",
                                     Options = new()
                                });
                            }
                        }
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
                Properties.Draw("ApplyAndSaveInputs");
                Properties.Draw("ShowDebug");

                GUI.enabled = false;
                EditorGUILayout.Toggle("Is Loaded", IsLoaded);
                GUI.enabled = true;
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}