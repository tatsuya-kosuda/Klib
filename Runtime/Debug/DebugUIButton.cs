using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace klib
{
    [RequireComponent(typeof(Button))]
    public class DebugUIButton : MonoBehaviour
    {

        [SerializeField]
        private string _label = "Label";

        private Text _labelText;

        private Button _button;

        public System.Action onClick;

        private bool _setLabel;

        private void Awake()
        {
            _button = GetComponent<Button>();

            if (_setLabel == false)
            {
                _labelText = GetComponentInChildren<Text>();
                _labelText.text = _label;
            }
        }

        public void SetLabel(string label)
        {
            _setLabel = true;

            if (_labelText == null) { _labelText = GetComponentInChildren<Text>(); }

            _labelText.text = label;
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnClickDebugButton);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveAllListeners();
        }

        private void OnClickDebugButton()
        {
            onClick.SafeInvoke();
        }

    }
}
