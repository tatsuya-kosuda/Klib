using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace klib
{
    public class DebugUISlider : MonoBehaviour
    {

        private Slider _slider;

        //private InputField _inputField;

        [SerializeField]
        private Text _valueText = null;

        [SerializeField]
        private string _label = "Label";

        [SerializeField]
        private Text _labelText = null;

        [SerializeField]
        private float _min = 0, _max = 1, _defaultValue = 0;

        public System.Action<float> onValueChanged;

        private bool _setDefaultValue;

        private bool _withoutCallback;

        private void Awake()
        {
            if (_setDefaultValue)
            {
                return;
            }

            _slider = GetComponentInChildren<Slider>();
            //_inputField = GetComponentInChildren<InputField>();
            _labelText.text = _label;
            _slider.minValue = _min;
            _slider.maxValue = _max;
            _slider.value = _defaultValue;
            _valueText.text = _defaultValue.ToString("F2");
        }

        private void OnEnable()
        {
            ///_inputField.onEndEdit.AddListener(OnEndInputFieldEdit);
            _slider.onValueChanged.AddListener(OnChangeSliderValue);
        }

        private void OnDisable()
        {
            //_inputField.onEndEdit.RemoveAllListeners();
            _slider.onValueChanged.RemoveAllListeners();
        }

        //private void OnEndInputFieldEdit(string text)
        //{
        //    if (float.TryParse(text, out float res))
        //    {
        //        _slider.value = res;
        //    }
        //}

        private void OnChangeSliderValue(float val)
        {
            _valueText.text = val.ToString("F2");

            if(_withoutCallback)
            {
                _withoutCallback = false;
                return;
            }

            onValueChanged.SafeInvoke(val);
        }

        public void SetDefaultValue(float val)
        {
            _setDefaultValue = true;

            if (_slider == null) { _slider = GetComponentInChildren<Slider>(); }

            //if (_inputField == null) { _inputField = GetComponentInChildren<InputField>(); }

            _slider.minValue = _min;
            _slider.maxValue = _max;
            _slider.value = val;
            _labelText.text = _label;
            _valueText.text = val.ToString("F2");
            _defaultValue = val;
        }

        public void SetLabel(string label)
        {
            _label = label;
            _labelText.text = label;
        }

        public void SetMinMax(Vector2 minmax)
        {
            if (_slider == null) { _slider = GetComponentInChildren<Slider>(); }

            _min = minmax.x;
            _max = minmax.y;
            _slider.minValue = _min;
            _slider.maxValue = _max;
        }

        public float GetValue()
        {
            if (_slider == null) { _slider = GetComponentInChildren<Slider>(); }

            float value;

            if (gameObject.activeInHierarchy == false)
            {
                value = _defaultValue;
            }
            else
            {
                value = _slider.value;
            }

            return value;
        }

        public void SetValueWithoutCallback(float value)
        {
            if (_slider == null) { _slider = GetComponentInChildren<Slider>(); }

            if (value == _slider.value) { return; }

            _withoutCallback = true;
            _slider.value = value;
        }

    }
}
