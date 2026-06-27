using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Farm
{
    public abstract class ConstructionPopupViewController : BaseViewController
    {
        [SerializeField] protected Button _closeButton;
        [SerializeField] protected RectTransform _frame;
        [SerializeField] protected float _yOffset = 60f;

        protected ConstructionController _target;

        public void Bind(ConstructionController construction)
        {
            if (construction == null)
            {
                Debug.LogError($"[{GetType().Name}] construction is null.", this);
                return;
            }

            _target = construction;
            OnBind();
            PositionAbove(construction.transform);
        }

        protected abstract void OnBind();

        public override void OnShow()
        {
            base.OnShow();

            if (_closeButton != null)
            {
                _closeButton.onClick.RemoveListener(OnCloseClicked);
                _closeButton.onClick.AddListener(OnCloseClicked);
            }
        }

        public override void OnHide()
        {
            if (_closeButton != null)
            {
                _closeButton.onClick.RemoveListener(OnCloseClicked);
            }

            base.OnHide();
        }

        protected void OnCloseClicked()
        {
            UIManager.Instance.HideTop();
        }

        protected void PositionAbove(Transform worldTarget)
        {
            if (worldTarget == null || _frame == null || Camera.main == null)
            {
                Debug.LogError($"[{GetType().Name}] _frame or Camera.main is missing.", this);
                return;
            }

            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                return;
            }

            Vector2 screenPos = Camera.main.WorldToScreenPoint(worldTarget.position);
            screenPos.y += _yOffset;

            // Overlay canvases convert with a null camera; world/camera canvases need theirs.
            Camera uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
            RectTransform canvasRect = canvas.transform as RectTransform;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, uiCamera, out Vector2 localPos))
            {
                _frame.anchoredPosition = localPos;
            }
        }
    }
}
