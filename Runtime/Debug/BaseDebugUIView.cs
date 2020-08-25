using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace klib
{
    [RequireComponent(typeof(CanvasGroup))]
    public class BaseDebugUIView : MonoBehaviour
    {

        protected List<System.IDisposable> _stream = new List<System.IDisposable>();

        private bool _isShowed;

        private CanvasGroup _rootCanvasGroup;

        private System.IDisposable _visibleStream;

        [SerializeField]
        private bool _isDefaultShow = false;

        private void Awake()
        {
            AfterAwake();
            _rootCanvasGroup = GetComponent<CanvasGroup>();
            _rootCanvasGroup.alpha = 0;
            _rootCanvasGroup.interactable = false;
            _rootCanvasGroup.blocksRaycasts = false;
            _visibleStream = UniRxUtils.ObserveInputKeyDown(KeyCode.F1, () =>
            {
                if (_isShowed)
                {
                    Hide();
                }
                else
                {
                    Show();
                }
            }, gameObject);

            if (_isDefaultShow)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        private void OnEnable()
        {
            Bind();
        }

        private void OnDisable()
        {
            UnBind();
        }

        protected virtual void AfterAwake()
        {
        }

        protected virtual void Bind()
        {
        }

        protected virtual void UnBind()
        {
            foreach (var stream in _stream)
            {
                stream.Dispose();
            }

            _stream.Clear();
        }

        private void Show()
        {
            gameObject.SetActive(true);
            _rootCanvasGroup.alpha = 1;
            _rootCanvasGroup.interactable = true;
            _rootCanvasGroup.blocksRaycasts = true;
            _isShowed = true;
        }

        private void Hide()
        {
            _rootCanvasGroup.alpha = 0;
            _rootCanvasGroup.interactable = false;
            _rootCanvasGroup.blocksRaycasts = false;
            _isShowed = false;
            gameObject.SetActive(false);
        }

    }
}
