using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Farm
{
    public class ConstructionBuildViewController : ConstructionPopupViewController
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _costText;
        [SerializeField] private Image _productIcon;
        [SerializeField] private Button _unlockButton;

        protected override void OnBind()
        {
            ConstructionConfig config = _target.Config;

            if (_nameText != null)
            {
                _nameText.text = config.DisplayName;
            }

            if (_costText != null)
            {
                _costText.text = NumberFormatter.Format(config.BuildCost);
            }

            if (_productIcon != null)
            {
                _productIcon.sprite = config.Icon;
                _productIcon.enabled = config.Icon != null;
            }

            if (_unlockButton != null)
            {
                _unlockButton.onClick.RemoveListener(OnUnlockClicked);
                _unlockButton.onClick.AddListener(OnUnlockClicked);
                _unlockButton.interactable = GameManager.Instance.Currency.CanAfford(config.BuildCost);
            }
        }

        public void OnUnlockClicked()
        {
            _target?.Build();
            UIManager.Instance.Hide<ConstructionBuildViewController>();
        }
    }
}
