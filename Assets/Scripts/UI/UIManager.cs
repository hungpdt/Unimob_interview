using System;
using System.Collections.Generic;
using UnityEngine;

namespace Farm
{
    [DefaultExecutionOrder(-90)]
    public class UIManager : MonoBehaviour
    {
        private static UIManager _instance;

        public static UIManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("[UIManager] Instance is null — UIManager is missing from the scene.");
                }
                return _instance;
            }
            private set { _instance = value; }
        }

        private readonly Dictionary<Type, BaseViewController> _views = new Dictionary<Type, BaseViewController>();
        private BaseViewController _current;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;

            BaseViewController[] views = GetComponentsInChildren<BaseViewController>(true);
            foreach (BaseViewController view in views)
            {
                _views[view.GetType()] = view;
                view.gameObject.SetActive(false);
            }
        }

        public T Show<T>() where T : BaseViewController
        {
            Type t = typeof(T);
            if (!_views.TryGetValue(t, out BaseViewController view))
            {
                Debug.LogError($"[UIManager] View {t.Name} not found — add it as a child of UIManager.", this);
                return null;
            }

            if (_current != null && _current != view)
            {
                _current.OnHide();
            }

            _current = view;
            _current.OnShow();
            return (T)view;
        }

        public void Hide<T>() where T : BaseViewController
        {
            if (_current is T)
            {
                HideTop();
            }
        }

        public void HideTop()
        {
            if (_current == null)
            {
                return;
            }

            _current.OnHide();
            _current = null;
        }

        public bool IsOpen<T>() where T : BaseViewController
        {
            return _current is T;
        }
    }
}
