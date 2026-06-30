using UnityEngine;
using UnityEngine.UI;

namespace Farm
{
    public abstract class BaseViewController : MonoBehaviour
    {
        [SerializeField] protected Button _closeButton;

        public virtual void OnShow()
        {
            gameObject.SetActive(true);

            if (_closeButton == null)
            {
                Debug.LogError($"[{GetType().Name}] _closeButton is null.", this);
                return;
            }

            _closeButton.onClick.RemoveListener(OnCloseClicked);
            _closeButton.onClick.AddListener(OnCloseClicked);
        }

        public virtual void OnHide()
        {
            if (_closeButton != null)
            {
                _closeButton.onClick.RemoveListener(OnCloseClicked);
            }

            gameObject.SetActive(false);
        }

        protected virtual void OnCloseClicked()
        {
            UIManager.Instance.HideTop();
        }
    }
}
