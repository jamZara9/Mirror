using UnityEngine;
using UnityEditor;
using UHFPS.Tools;
using UHFPS.Scriptable;
using ThunderWire.Editors;
using static UHFPS.Scriptable.DialogueAsset;
using static UHFPS.Editors.AudioWaveformWrapper;

namespace UHFPS.Editors
{
    [CustomEditor(typeof(DialogueAsset))]
    public class DialogueAssetEditor : InspectorEditor<DialogueAsset>
    {
        private AudioWaveformWrapper waveformWrapper;
        private WaveformPreview waveformPreview;

        private int draggedIndex = -1;
        private int selectedDialogue = -1;
        private int selectedSubtitle = -1;

        public override void OnEnable()
        {
            base.OnEnable();
            waveformWrapper = new AudioWaveformWrapper();
        }

        private void OnDestroy()
        {
            AudioUtilWrapper.StopAllPreviewClips();
            if (waveformPreview != null)
            {
                waveformPreview.Dispose();
                waveformPreview = null;
            }

            ResetIndexes();
        }

        private void ResetIndexes()
        {
            draggedIndex = -1;
            selectedDialogue = -1;
            selectedSubtitle = -1;
        }

        public override void OnInspectorGUI()
        {
            EditorDrawing.DrawInspectorHeader(new GUIContent("Dialogue Asset"));
            EditorGUILayout.Space();

            serializedObject.Update();
            {
                // Draw dialogues list
                int arraySize = EditorDrawing.BeginDrawCustomList(Properties["Dialogues"], new GUIContent("Dialogues"));
                {
                    for (int i = 0; i < arraySize; i++)
                    {
                        Rect rect = EditorGUILayout.GetControlRect(false, 20f);

                        if (DrawButton(rect, new GUIContent($"Dialogue {i}"), selectedDialogue == i))
                        {
                            if (selectedDialogue != i)
                            {
                                AudioUtilWrapper.StopAllPreviewClips();
                                ResetIndexes();

                                if (waveformPreview != null)
                                {
                                    waveformPreview.Dispose();
                                    waveformPreview = null;
                                }
                            }

                            selectedDialogue = i;
                        }

                        Rect minusRect = rect;
                        minusRect.xMin = minusRect.xMax - EditorGUIUtility.singleLineHeight;
                        minusRect.y += 2f;

                        if (GUI.Button(minusRect, EditorUtils.Styles.MinusIcon, EditorStyles.iconButton))
                        {
                            AudioUtilWrapper.StopAllPreviewClips();

                            Properties["Dialogues"].DeleteArrayElementAtIndex(i);
                            serializedObject.ApplyModifiedProperties();
                            ResetIndexes();
                        }
                    }
                }
                EditorDrawing.EndDrawCustomList(new GUIContent("Add Dialogue"), true, () =>
                {
                    Target.Dialogues.Add(new());
                    serializedObject.ApplyModifiedProperties();
                });

                // Draw selected dialogue properties
                if(selectedDialogue >= 0)
                {
                    EditorGUILayout.Space();
                    GUIContent title = new($"Dialogue {selectedDialogue}");
                    using (new EditorDrawing.BorderBoxScope(title, roundedBox: false))
                    {
                        SerializedProperty dialogue = Properties["Dialogues"].GetArrayElementAtIndex(selectedDialogue);
                        SerializedProperty audioClip = dialogue.FindPropertyRelative("DialogueAudio");
                        SerializedProperty subtitleType = dialogue.FindPropertyRelative("SubtitleType");

                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(audioClip);
                        EditorGUILayout.PropertyField(subtitleType);
                        SubtitleTypeEnum subType = (SubtitleTypeEnum)subtitleType.enumValueIndex;

                        if (EditorGUI.EndChangeCheck())
                        {
                            AudioUtilWrapper.StopAllPreviewClips();
                            if (waveformPreview != null)
                            {
                                waveformPreview.Dispose();
                                waveformPreview = null;
                            }
                        }

                        GUIContent[] audioContent = {
                                EditorGUIUtility.IconContent("PlayButton"),
                                EditorGUIUtility.IconContent("PauseButton"),
                                EditorGUIUtility.IconContent("Refresh"),
                        };

                        GUIStyle audioButtons = new(GUI.skin.button)
                        {
                            fixedHeight = 0,
                            fixedWidth = 25
                        };

                        // Draw single dialogue properties
                        if (subType == SubtitleTypeEnum.Single)
                        {
                            if (Target.Dialogues[selectedDialogue].SingleSubtitle == null)
                            {
                                Target.Dialogues[selectedDialogue].SingleSubtitle = new();
                                return;
                            }

                            SerializedProperty subtitle = dialogue.FindPropertyRelative("SingleSubtitle");
                            PropertyCollection subtitleP = EditorDrawing.GetAllProperties(subtitle);

                            EditorGUILayout.Space();
                            Rect toolbarRect = EditorGUILayout.GetControlRect(false, 20);
                            toolbarRect.width = 20 * audioContent.Length;
                            toolbarRect.x = EditorGUIUtility.currentViewWidth / 2 - toolbarRect.width / 2;

                            AudioClip clip = audioClip.objectReferenceValue != null
                                ? (AudioClip)audioClip.objectReferenceValue
                                : null;

                            // Draw audio play controls
                            using (new EditorGUI.DisabledGroupScope(clip == null))
                            {
                                int select = GUI.Toolbar(toolbarRect, -1, audioContent, audioButtons);
                                if (select >= 0)
                                {
                                    switch (select)
                                    {
                                        case 0:
                                            if (AudioUtilWrapper.IsPreviewClipPlaying())
                                            {
                                                AudioUtilWrapper.ResumePreviewClip();
                                            }
                                            else
                                            {
                                                AudioUtilWrapper.PlayPreviewClip(clip);
                                            }
                                            break;
                                        case 1:
                                            AudioUtilWrapper.PausePreviewClip();
                                            break;
                                        case 2:
                                            AudioUtilWrapper.StopAllPreviewClips();
                                            break;
                                    }
                                }
                            }

                            float clipPosition = 0f;
                            float audioLength = clip != null ? clip.length : 0;
                            float maxLengthRounded = (float)System.Math.Round(audioLength, 3);

                            if (AudioUtilWrapper.IsPreviewClipPlaying())
                            {
                                clipPosition = AudioUtilWrapper.GetPreviewClipPosition();
                                clipPosition = (float)System.Math.Round(clipPosition, 3);
                                Repaint();
                            }

                            GUIStyle clipPosStyle = new(EditorStyles.miniBoldLabel);
                            clipPosStyle.alignment = TextAnchor.MiddleCenter;

                            // Draw audio play position
                            EditorGUILayout.LabelField($"{clipPosition}/{maxLengthRounded}", clipPosStyle);
                            EditorGUILayout.Space(2f);

                            GUIContent headerContent = EditorDrawing.IconTextContent($"Single Subtitle", "TextAsset Icon", 12f);
                            using (new EditorDrawing.BorderBoxScope(headerContent))
                            {
                                EditorDrawing.ResetIconSize();

                                using (new EditorGUI.DisabledGroupScope(audioClip.objectReferenceValue == null))
                                {
                                    EditorGUILayout.Slider(subtitleP["Time"], 0, audioLength, new GUIContent("Start Time"));
                                }

                                subtitleP.Draw("Narrator");
                                subtitleP.Draw("NarratorColor");
                                subtitleP.Draw("Text");
                            }

                            if(clip == null)
                            {
                                EditorGUILayout.Space();
                                EditorGUILayout.HelpBox("Dialogue Audio is missing! Please assign it to make the subtitles work properly.", MessageType.Warning);
                            }
                        }
                        else if (audioClip.objectReferenceValue == null)
                        {
                            EditorGUILayout.Space();
                            EditorGUILayout.HelpBox("Assign a Dialogie Audio to display the audio waveform and start setting the subtitles.", MessageType.Warning);
                        }
                        else
                        {
                            // Draw multiple dialogue properties
                            AudioClip clip = (AudioClip)audioClip.objectReferenceValue;

                            // Draw help box
                            EditorGUILayout.Space();
                            using (new EditorDrawing.BackgroundColorScope("#F7E987"))
                            {
                                EditorGUILayout.LabelField("You can move the subtitle by dragging the vertical line.", EditorStyles.helpBox);
                                EditorGUILayout.LabelField("To select a subtitle, click the button below the vertical line.", EditorStyles.helpBox);
                                EditorGUILayout.LabelField("<b>Important:</b> Make sure that the order of the subtitles is ascending! (0, 1, 2 ..)", EditorDrawing.Styles.RichHelpBox);
                            }

                            EditorGUILayout.Space();
                            EditorGUILayout.Space();

                            Rect toolbarButtonsRect = EditorGUILayout.GetControlRect(false, 20);
                            toolbarButtonsRect.width = 20 * audioContent.Length;
                            toolbarButtonsRect.x = EditorGUIUtility.currentViewWidth / 2 - toolbarButtonsRect.width / 2;

                            // Draw audio play controls
                            int select = GUI.Toolbar(toolbarButtonsRect, -1, audioContent, audioButtons);
                            if(select >= 0)
                            {
                                switch (select)
                                {
                                    case 0:
                                        if (AudioUtilWrapper.IsPreviewClipPlaying())
                                        {
                                            AudioUtilWrapper.ResumePreviewClip();
                                        }
                                        else
                                        {
                                            AudioUtilWrapper.PlayPreviewClip(clip);
                                        }
                                        break;
                                    case 1:
                                        AudioUtilWrapper.PausePreviewClip();
                                        break;
                                    case 2:
                                        AudioUtilWrapper.StopAllPreviewClips();
                                        break;
                                }
                            }

                            Rect addSubtitleRect = EditorGUILayout.GetControlRect(false, 20);
                            addSubtitleRect.width = 100f;
                            addSubtitleRect.x = 7 + EditorGUIUtility.currentViewWidth / 2 - addSubtitleRect.width / 2;

                            using (new EditorDrawing.BackgroundColorScope("#F7E987"))
                            {
                                if (GUI.Button(addSubtitleRect, "Add Subtitle"))
                                {
                                    Target.Dialogues[selectedDialogue].Subtitles.Add(new());
                                    serializedObject.ApplyModifiedProperties();
                                }
                            }

                            EditorGUILayout.Space(2f);

                            Rect rect = EditorGUILayout.GetControlRect(false, 100f);
                            rect.xMin += 10f;
                            rect.xMax -= 10f;

                            Rect waveRect = new Rect(rect.x, rect.y, rect.width, 80f);
                            Rect quantizedRect = new(Mathf.Ceil(waveRect.x), Mathf.Ceil(waveRect.y), Mathf.Ceil(waveRect.width), Mathf.Ceil(waveRect.height));

                            if (Event.current.type == EventType.Repaint)
                            {
                                if (waveformPreview == null)
                                {
                                    // Initialize audio waveform
                                    waveformPreview = waveformWrapper.Create((int)quantizedRect.width, clip);
                                    waveformPreview.backgroundColor = Color.black.Alpha(0.15f);
                                    waveformPreview.waveColor = Color.yellow;
                                    waveformPreview.SetChannelMode(WaveformPreview.ChannelMode.MonoSum);
                                    waveformPreview.OptimizeForSize(quantizedRect.size);
                                }
                                else
                                {
                                    // Draw audio waveform
                                    waveformPreview.ApplyModifications();
                                    waveformPreview.Render(quantizedRect);
                                }
                            }

                            // Draw audio waveform controls
                            DrawClipPreviewControls(quantizedRect, clip);

                            EditorGUILayout.Space();
                            DrawSubtitleProperties(dialogue, clip);
                        }
                    }
                }
            }
            serializedObject.ApplyModifiedProperties();
        }

        private bool DrawButton(Rect rect, GUIContent title, bool selected)
        {
            ColorUtility.TryParseHtmlString("#2D2D2D", out Color normalColor);
            ColorUtility.TryParseHtmlString("#4D4D4D", out Color hoverColor);
            ColorUtility.TryParseHtmlString("#3B3629", out Color selectedColor);
            Color bgColor = selected ? selectedColor : normalColor;

            Rect hoverRect = rect;
            hoverRect.xMax -= EditorGUIUtility.singleLineHeight + 2f;

            Event e = Event.current;
            if (hoverRect.Contains(e.mousePosition))
            {
                bgColor = hoverColor;
                if (e.type == EventType.MouseDown && e.button == 0)
                {
                    selected = true;
                    bgColor = normalColor;
                    e.Use();
                }
            }

            EditorGUI.DrawRect(rect, bgColor);
            rect.xMin += EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.LabelField(rect, title);

            return selected;
        }

        private void DrawClipPreviewControls(Rect rect, AudioClip clip)
        {
            Event current = Event.current;
            Dialogue dialogue = Target.Dialogues[selectedDialogue];

            int audioSamples = clip.samples / (int)rect.width;
            float audioLength = rect.width / clip.length;

            // Draw dialogue subtitles preview
            for (int i = 0; i < dialogue.Subtitles.Count; i++)
            {
                var subtitle = dialogue.Subtitles[i];
                float selectionWidth = 10f;

                float position = subtitle.Time;
                float lineX = rect.x + ((int)(audioLength * position));

                Rect lineRect = new Rect(lineX, rect.y, 1f, rect.height);
                Rect selectionBox = lineRect;
                selectionBox.width = selectionWidth;
                selectionBox.x -= selectionWidth / 2f;

                Color rectColor = Color.white;
                bool flag = draggedIndex == -1 || draggedIndex == i;
                if (flag && selectionBox.Contains(current.mousePosition))
                {
                    rectColor = Color.red;
                    if (current.type == EventType.MouseDown && current.button == 0)
                    {
                        draggedIndex = i;
                        current.Use();
                    }
                }

                EditorGUI.DrawRect(lineRect, rectColor);

                Rect selectButtonRect = lineRect;
                selectButtonRect.width = 20f;
                selectButtonRect.height = 20f;
                selectButtonRect.y = lineRect.yMax + 1f;
                selectButtonRect.x -= 9f;

                GUIStyle buttonStlye = new GUIStyle(GUI.skin.button);
                buttonStlye.alignment = TextAnchor.MiddleCenter;

                if(GUI.Toggle(selectButtonRect, selectedSubtitle == i, i.ToString(), buttonStlye))
                {
                    if (selectedSubtitle != i)
                        GUI.FocusControl(null);

                    selectedSubtitle = i;
                }
            }

            // Handle audio subtitle preview position
            if (current.type == EventType.MouseUp)
            {
                draggedIndex = -1;
            }
            else if (draggedIndex >= 0 && current.type == EventType.MouseDrag)
            {
                var subtitle = dialogue.Subtitles[draggedIndex];
                float newTime = (current.mousePosition.x - rect.x) / audioLength;
                subtitle.Time = Mathf.Clamp(newTime, 0f, clip.length);
                current.Use();
            }

            // Handle Mouse Drag and Mouse Down to set audio clip position
            if (draggedIndex < 0 && (current.type == EventType.MouseDrag || current.type == EventType.MouseDown) && rect.Contains(current.mousePosition))
            {
                AudioUtilWrapper.StopAllPreviewClips();
                AudioUtilWrapper.PlayPreviewClip(clip);

                int relativeMouseX = (int)(current.mousePosition.x - rect.x);
                int samplePos = audioSamples * relativeMouseX;

                AudioUtilWrapper.SetPreviewClipSamplePosition(clip, samplePos);
                current.Use();
            }

            if (AudioUtilWrapper.IsPreviewClipPlaying())
            {
                // Draw the preview line of the audio clip
                if (current.type == EventType.Repaint)
                {
                    float clipPosition = AudioUtilWrapper.GetPreviewClipPosition();
                    float lineX = rect.x + ((int)(audioLength * clipPosition));
                    EditorGUI.DrawRect(new Rect(lineX, rect.y, 2f, rect.height), Color.green);
                }
            }

            Repaint();
        }
    
        private void DrawSubtitleProperties(SerializedProperty dialogue, AudioClip clip)
        {
            if (selectedDialogue < 0 || selectedSubtitle < 0)
                return;

            EditorGUILayout.Space();
            SerializedProperty subtitles = dialogue.FindPropertyRelative("Subtitles");
            SerializedProperty subtitle = subtitles.GetArrayElementAtIndex(selectedSubtitle);
            PropertyCollection subtitleP = EditorDrawing.GetAllProperties(subtitle);

            GUIContent headerContent = EditorDrawing.IconTextContent($"Subtitle {selectedSubtitle}", "TextAsset Icon", 12f);
            Rect headerRect = EditorDrawing.BeginHeaderBorderLayout(headerContent);
            {
                EditorDrawing.ResetIconSize();

                EditorGUILayout.Slider(subtitleP["Time"], 0, clip.length);
                subtitleP.Draw("Narrator");
                subtitleP.Draw("NarratorColor");
                subtitleP.Draw("Text");
            }
            EditorDrawing.EndBorderHeaderLayout();

            Rect deleteRect = headerRect;
            deleteRect.xMin = deleteRect.xMax - EditorGUIUtility.singleLineHeight;
            deleteRect.y += 3f;
            deleteRect.x -= 2f;

            GUIContent deleteIcon = EditorDrawing.IconContent("TreeEditor.Trash");
            deleteIcon.tooltip = "Remove Subtitle";

            if(GUI.Button(deleteRect, deleteIcon, EditorStyles.iconButton))
            {
                subtitles.DeleteArrayElementAtIndex(selectedSubtitle);
                selectedSubtitle = -1;
            }
        }
    }
}