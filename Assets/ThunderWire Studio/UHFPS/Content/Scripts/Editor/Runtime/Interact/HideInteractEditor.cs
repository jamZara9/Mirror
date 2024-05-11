using System;
using UnityEngine;
using UnityEditor;
using UHFPS.Runtime;
using Cinemachine;
using ThunderWire.Editors;

namespace UHFPS.Editors
{
    [CustomEditor(typeof(HideInteract))]
    public class HideInteractEditor : InspectorEditor<HideInteract>
    {
        private string[] blendStyles;

        public override void OnEnable()
        {
            base.OnEnable();
            blendStyles = Enum.GetNames(typeof(CinemachineBlendDefinition.Style))[1..];
        }

        public override void OnInspectorGUI()
        {
            EditorDrawing.DrawInspectorHeader(new GUIContent("Hide Interact"), Target);
            EditorGUILayout.Space();

            serializedObject.Update();
            {
                Properties.Draw("HideStyle");
                Properties.Draw("DrawGizmos");
                EditorGUILayout.Space();

                using(new EditorDrawing.BorderBoxScope(new GUIContent("Hide Positioning")))
                {
                    EditorGUILayout.HelpBox("The hiding position represents the position where the player will be teleported to, so it does not represent the actual hiding position.", MessageType.Info);
                    EditorGUILayout.Space();

                    Properties.Draw("PlayerHidePosition");
                    Properties.Draw("PlayerUnhidePosition");
                }

                EditorGUILayout.Space();
                using (new EditorDrawing.BorderBoxScope(new GUIContent("Hide References")))
                {
                    Properties.Draw("VirtualCamera");
                    Properties.Draw("Animator");
                    Properties.Draw("UnhideText");
                }

                EditorGUILayout.Space();
                using (new EditorDrawing.BorderBoxScope(new GUIContent("Blend Settings")))
                {
                    SerializedProperty blendDefinition = Properties["BlendDefinition"];
                    SerializedProperty style = blendDefinition.FindPropertyRelative("m_Style");
                    SerializedProperty time = blendDefinition.FindPropertyRelative("m_Time");
                    SerializedProperty curve = blendDefinition.FindPropertyRelative("m_CustomCurve");

                    CinemachineBlendDefinition.Style blendStyle = (CinemachineBlendDefinition.Style)style.enumValueIndex;

                    style.enumValueIndex = EditorGUILayout.Popup(new GUIContent("Style"), style.enumValueIndex - 1, blendStyles) + 1;
                    EditorGUILayout.PropertyField(time);

                    if (blendStyle == CinemachineBlendDefinition.Style.Custom)
                        EditorGUILayout.PropertyField(curve);

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Blend Offsets", EditorStyles.miniBoldLabel);
                    Properties.Draw("BlendInOffset");
                    Properties.Draw("BlendOutOffset");
                }

                EditorGUILayout.Space();
                using (new EditorDrawing.BorderBoxScope(new GUIContent("Animation Settings")))
                {
                    Properties.Draw("HideParameter");
                    Properties.Draw("DefaultStateName");
                    Properties.Draw("HideStateName");
                    Properties.Draw("UnhideStateName");
                }

                EditorGUILayout.Space();
                if (EditorDrawing.BeginFoldoutBorderLayout(Properties["OnHideStart"], new GUIContent("Events")))
                {
                    Properties.Draw("OnHideStart");
                    Properties.Draw("OnHidden");
                    Properties.Draw("OnUnhide");
                    EditorDrawing.EndBorderHeaderLayout();
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}