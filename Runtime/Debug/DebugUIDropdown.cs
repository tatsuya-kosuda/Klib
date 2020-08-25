using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace klib
{
    public class DebugUIDropdown : MonoBehaviour
    {

        [SerializeField]
        private string _label = "Label";

        [SerializeField]
        private Text _labelText = null;

        private Dropdown _dropDown;

        public System.Action<int> onValueChanged;

        private bool _setLabel;

        private bool _setDefaultValue;

        [SerializeField]
        private int _defaultValue = 0;

        private void Awake()
        {
            if (_setLabel == false) { _labelText.text = _label; }

            if (_setDefaultValue == false)
            {
                _dropDown = GetComponentInChildren<Dropdown>();
                _dropDown.value = _defaultValue;
            }
        }

        private void OnEnable()
        {
            _dropDown.onValueChanged.AddListener(OnDropdownValueChanged);
        }

        private void OnDisable()
        {
            _dropDown.onValueChanged.RemoveAllListeners();
        }

        private void OnDropdownValueChanged(int val)
        {
            onValueChanged.SafeInvoke(val);
        }

        public void SetLabel(string label)
        {
            _setLabel = true;
            _labelText.text = label;
        }

        public void SetDefaultValue(int value)
        {
            _setDefaultValue = true;

            if (_dropDown == null) { _dropDown = GetComponentInChildren<Dropdown>(); }

            _dropDown.value = value;
        }

        public Dropdown.OptionData GetOptionData(int index)
        {
            return _dropDown.options[Mathf.Clamp(index, 0, _dropDown.options.Count - 1)];
        }

    }
}
