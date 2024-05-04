using System;
using System.Linq;
using UnityEngine.Events;
using UnityEngine;
using ThunderWire.Attributes;
using UHFPS.Tools;
using TMPro;

namespace UHFPS.Runtime
{
    [InspectorHeader("Options Radio")]
    public class OptionsRadio : OptionBehaviour
    {
        public TMP_Text RadioText;
        public uint Current = 0;

        [Header("Radio Options")]
        public bool IsCustomData;
        public GString[] Options;

        [Header("Events")]
        public UnityEvent<int> OnChange;

        private void Start()
        {
            if (IsCustomData)
                return;

            bool listenToChange = false;
            for (int i = 0; i < Options.Length; i++)
            {
                Options[i].SubscribeGloc(text =>
                {
                    if (!listenToChange)
                        return;

                    int index = i;
                    if (index == Current)
                        RadioText.text = text;
                });
            }

            SetOption((int)Current);
            listenToChange = true;
        }

        public void ChangeOption(int change)
        {
            int nextOption = GameTools.Wrap((int)Current + change, 0, Options.Length);
            SetOption(nextOption);
        }

        public void SetOption(int index)
        {
            Current = (uint)index;
            RadioText.text = Options[Current];
            OnChange?.Invoke((int)Current);
            IsChanged = true;
        }

        public override void SetOptionData(string[] data)
        {
            Options = new GString[0];
            Options = data.Select(x => new GString(x)).ToArray();
            RadioText.text = Options[Current];
        }

        public override object GetOptionValue()
        {
            return (int)Current;
        }

        public override void SetOptionValue(object value)
        {
            int radio = Convert.ToInt32(value);
            SetOption(radio);
            IsChanged = false;
        }
    }
}