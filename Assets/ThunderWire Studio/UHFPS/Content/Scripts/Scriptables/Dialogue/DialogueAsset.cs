using System;
using System.Collections.Generic;
using UnityEngine;
using UHFPS.Runtime;

namespace UHFPS.Scriptable
{
    [CreateAssetMenu(fileName = "Dialogue", menuName = "UHFPS/Dialogue/Dialogue Asset")]
    public class DialogueAsset : ScriptableObject
    {
        public enum SubtitleTypeEnum { Single, Multiple }

        [Serializable]
        public sealed class DialogueSubtitle
        {
            public float Time;
            public string Narrator;
            public Color NarratorColor = Color.white;
            public GString Text;
        }

        [Serializable]
        public sealed class Dialogue
        {
            public AudioClip DialogueAudio;
            public SubtitleTypeEnum SubtitleType;

            public DialogueSubtitle SingleSubtitle = new();
            public List<DialogueSubtitle> Subtitles;

            public Dialogue()
            {
                DialogueAudio = null;
                Subtitles = new();
            }

            public Dialogue Copy()
            {
                Dialogue copy = new()
                {
                    DialogueAudio = DialogueAudio,
                    SubtitleType = SubtitleType,
                    Subtitles = new(),

                    SingleSubtitle = new()
                    {
                        Time = SingleSubtitle.Time,
                        Narrator = SingleSubtitle.Narrator,
                        NarratorColor = SingleSubtitle.NarratorColor,
                        Text = new(SingleSubtitle.Text),
                    }
                };

                foreach (var subtitle in Subtitles)
                {
                    copy.Subtitles.Add(new()
                    {
                        Time = subtitle.Time,
                        Narrator = subtitle.Narrator,
                        NarratorColor = subtitle.NarratorColor,
                        Text = new(subtitle.Text)
                    });
                }

                return copy;
            }
        }

        public List<Dialogue> Dialogues = new();
    }
}