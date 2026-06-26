using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Farm
{
    public class ConstructionBuildViewController : BaseViewController
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _costText;
        [SerializeField] private Image _productIcon;
        [SerializeField] private Button _unlockButton;
        [SerializeField] private Button _closeButton;

        // The visible popup box (child of the canvas). Screen Space Overlay canvas root
        // cannot be moved, so we reposition this Frame instead.
        [SerializeField] private RectTransform _frame;

        // Extra screen pixels above the box so the pointer sits right on top of it.
        [SerializeField] private float _yOffset = 60f;

        private BoxController _target;

        public void Bind(BoxController box)
        {
            if (box == null)
            {
                Debug.LogError("[ConstructionBuildViewController] box is null.", this);
                return;
            }

            _target = box;

            if (_nameText != null)
            {
                _nameText.text = box.Config.DisplayName;
            }

            if (_costText != null)
            {
                _costText.text = NumberFormatter.Format(box.Config.BuildCost);
            }

            if (_productIcon != null)
            {
                _productIcon.sprite = box.Config.Icon;
                _productIcon.enabled = box.Config.Icon != null;
            }

            if (_unlockButton != null)
            {
                _unlockButton.onClick.RemoveListener(OnUnlockClicked);
                _unlockButton.onClick.AddListener(OnUnlockClicked);
                _unlockButton.interactable = GameManager.Instance.Currency.CanAfford(box.Config.BuildCost);
            }

            PositionAbove(box.transform);
        }

        public override void OnShow()
        {
            base.OnShow();

            if (_closeButton != null)
            {
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

        public void OnUnlockClicked()
        {
            _target?.Build();
            UIManager.Instance.Hide<ConstructionBuildViewController>();
        }

        public void OnCloseClicked()
        {
            UIManager.Instance.Hide<ConstructionBuildViewController>();
        }

        // Move the Frame to sit above the box on screen.
        // Works for Screen Space Overlay (camera = null) and Camera/World canvases.
        private void PositionAbove(Transform worldTarget)
        {
            if (worldTarget == null || _frame == null || Camera.main == null)
            {
                Debug.LogError("[ConstructionBuildViewController] _frame or Camera.main is missing.", this);
                return;
            }

            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                return;
            }

            Vector2 screenPos = Camera.main.WorldToScreenPoint(worldTarget.position);
            screenPos.y += _yOffset;

            Camera uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
            RectTransform canvasRect = canvas.transform as RectTransform;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, uiCamera, out Vector2 localPos))
            {
                _frame.anchoredPosition = localPos;
            }
        }
    }
}
