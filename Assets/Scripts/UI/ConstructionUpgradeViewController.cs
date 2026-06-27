using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Farm
{
    public class ConstructionUpgradeViewController : ConstructionPopupViewController
    {
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _costText;
        [SerializeField] private Slider _progressSlider;
        [SerializeField] private Button _upgradeButton;
        [SerializeField] private GameObject _maxLabel;

        private ConstructionController _subscribed;

        protected override void OnBind()
        {
            if (_upgradeButton != null)
            {
                _upgradeButton.onClick.RemoveListener(OnUpgradeClicked);
                _upgradeButton.onClick.AddListener(OnUpgradeClicked);
            }

            Subscribe(_target);
            Refresh();
        }

        public override void OnHide()
        {
            Subscribe(null);
            base.OnHide();
        }

        private void Subscribe(ConstructionController next)
        {
            if (_subscribed == next)
            {
                return;
            }

            if (_subscribed != null)
            {
                _subscribed.OnStatsChanged -= OnTargetStatsChanged;
            }

            _subscribed = next;

            if (_subscribed != null)
            {
                _subscribed.OnStatsChanged += OnTargetStatsChanged;
            }
        }

        private void OnUpgradeClicked()
        {
            if (_target == null)
            {
                return;
            }

            _target.TryUpgrade();
            Refresh();
        }

        private void OnTargetStatsChanged(ConstructionController construction)
        {
            Refresh();
        }

        private void Refresh()
        {
            if (_target == null || _target.Config == null)
            {
                Debug.LogError($"[{GetType().Name}] _target or its Config is null.", this);
                return;
            }

            ConstructionConfig config = _target.Config;

            if (_levelText != null)
            {
                _levelText.text = $"Level {_target.Level}";
            }

            if (_nameText != null)
            {
                _nameText.text = config.DisplayName;
            }

            if (_progressSlider != null && _target.MaxLevel > 0)
            {
                _progressSlider.value = _target.Level / (float)_target.MaxLevel;
            }

            if (_target.IsMaxLevel)
            {
                ShowMaxLevel();
            }
            else
            {
                ShowUpgradeable(config);
            }
        }

        private void ShowMaxLevel()
        {
            if (_maxLabel != null)
            {
                _maxLabel.SetActive(true);
            }

            if (_costText != null)
            {
                _costText.gameObject.SetActive(false);
            }

            if (_upgradeButton != null)
            {
                _upgradeButton.gameObject.SetActive(false);
            }
        }

        private void ShowUpgradeable(ConstructionConfig config)
        {
            if (_maxLabel != null)
            {
                _maxLabel.SetActive(false);
            }

            ConstructionConfig.UpgradeLevel next = config.Levels[_target.Level];

            if (_costText != null)
            {
                _costText.gameObject.SetActive(true);
                _costText.text = NumberFormatter.Format(next.Cost);
            }

            if (_upgradeButton != null)
            {
                _upgradeButton.gameObject.SetActive(true);
                _upgradeButton.interactable = GameManager.Instance.Currency.CanAfford(next.Cost);
            }
        }
    }
}
