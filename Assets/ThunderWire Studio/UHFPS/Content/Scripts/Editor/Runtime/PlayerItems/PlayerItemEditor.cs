using UnityEngine;
using UnityEditor;
using UHFPS.Runtime;
using UHFPS.Scriptable;
using ThunderWire.Editors;

namespace UHFPS.Editors
{
    public class PlayerItemEditor<T> : Editor where T : PlayerItemBehaviour
    {
        public T Target { get; private set; }
        public PlayerItemBehaviour Behaviour { get; private set; }
        public PropertyCollection Properties { get; private set; }

        private SerializedProperty foldoutProperty;
        private MotionListHelper motionListHelper;
        private ExternalMotionsDrawer externalMotionsDrawer;

        public virtual void OnEnable()
        {
            Target = target as T;
            Behaviour = Target;

            Properties = EditorDrawing.GetAllProperties(serializedObject);
            foldoutProperty = Properties["<ItemObject>k__BackingField"];

            MotionPreset preset = Behaviour.MotionPreset;
            motionListHelper = new(preset);

            SerializedProperty externalMotions = Properties["ExternalMotions"];
            externalMotionsDrawer = new(externalMotions, Target.ExternalMotions);
        }

        public override void OnInspectorGUI()
        {
            GUIContent headerContent = EditorDrawing.IconTextContent("Wieldable Settings", "Settings");
            EditorDrawing.SetLabelColor("#E0FBFC");

            if (EditorDrawing.BeginFoldoutBorderLayout(foldoutProperty, headerContent))
            {
                EditorDrawing.ResetLabelColor();

                if (EditorDrawing.BeginFoldoutToggleBorderLayout(new GUIContent("Wall Detection"), Properties["EnableWallDetection"]))
                {
                    using (new EditorGUI.DisabledGroupScope(!Properties.BoolValue("EnableWallDetection")))
                    {
                        Properties.Draw("WallHitTransform");
                        Properties.Draw("WallHitMask");
                        Properties.Draw("WallHitRayDistance");
                        Properties.Draw("WallHitRayRadius");
                        Properties.Draw("WallHitRayOffset");

                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
                        Properties.Draw("WallHitAmount");
                        Properties.Draw("WallHitTime");
                        Properties.Draw("ShowRayGizmos");
                    }
                    EditorDrawing.EndBorderHeaderLayout();
                }

                if (EditorDrawing.BeginFoldoutToggleBorderLayout(new GUIContent("Wieldable Motions"), Properties["EnableMotionPreset"]))
                {
                    using (new EditorGUI.DisabledGroupScope(!Properties.BoolValue("EnableMotionPreset")))
                    {
                        motionListHelper.DrawMotionPresetField(Properties["MotionPreset"]);
                        Properties.DrawBacking("MotionPivot");

                        if (motionListHelper != null)
                        {
                            EditorGUILayout.Space();
                            MotionPreset presetInstance = Behaviour.MotionBlender.Instance;
                            motionListHelper.DrawMotionsList(presetInstance);
                        }
                    }
                    EditorDrawing.EndBorderHeaderLayout();
                }

                if (EditorDrawing.BeginFoldoutToggleBorderLayout(new GUIContent("Camera Motions"), Properties["EnableExternalMotion"]))
                {
                    using (new EditorGUI.DisabledGroupScope(!Properties.BoolValue("EnableExternalMotion")))
                    {
                        externalMotionsDrawer.DrawExternalMotions(new GUIContent("External Motions"));
                    }
                    EditorDrawing.EndBorderHeaderLayout();
                }

                EditorDrawing.EndBorderHeaderLayout();
            }
            EditorDrawing.ResetLabelColor();
        }
    }
}