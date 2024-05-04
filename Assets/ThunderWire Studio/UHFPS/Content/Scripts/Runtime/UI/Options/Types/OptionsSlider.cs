using System;
using UnityEngine;
using UnityEngine.UI;
using ThunderWire.Attributes;
using TMPro;
using static UnityEngine.Rendering.DebugUI;

namespace UHFPS.Runtime
{
    [InspectorHeader("Options Slider")]
    public class OptionsSlider : OptionBehaviour
    {
        public enum SliderTypeEnum { FloatSlider, IntegerSlider }

        public Slider Slider;
        public TMP_Text SliderText;

        [Header("Slider Settings")]
        public SliderTypeEnum SliderType = SliderTypeEnum.FloatSlider;
        public MinMax SliderLimits = new(0, 1);
        public float SliderValue = 0f;

        [Header("Snap Settings")]
        public bool UseSnapping;
        public float SnapValue = 0.05f;

        private void Start()
        {
            float value = SliderValue;
            Slider.wholeNumbers = SliderType == SliderTypeEnum.IntegerSlider;
            Slider.minValue = SliderLimits.RealMin;
            Slider.maxValue = SliderLimits.RealMax;

            Slider.value = value;
            SliderText.text = SliderValue.ToString();
        }

        public void SetSliderValue(float value)
        {
            if (SliderType == SliderTypeEnum.FloatSlider)
                SliderValue = (float)Math.Round(value, 2);
            else if (SliderType == SliderTypeEnum.IntegerSlider)
                SliderValue = Mathf.RoundToInt(value);

            if(UseSnapping) 
                SliderValue = SnapTo(SliderValue, SnapValue);

            SliderText.text = SliderValue.ToString();
            IsChanged = true;
        }

        private float SnapTo(float value, float multiple)
        {
            return Mathf.Round(value / multiple) * multiple;
        }

        public override object GetOptionValue()
        {
            return SliderType switch
            {
                SliderTypeEnum.FloatSlider => SliderValue,
                SliderTypeEnum.IntegerSlider => Mathf.RoundToInt(SliderValue),
                _ => SliderValue
            };
        }

        public override void SetOptionValue(object value)
        {
            SetSliderValue((float)value);
            Slider.value = SliderValue;
            IsChanged = false;
        }
    }
}