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
        [SerializeField] private TMP_Text _secondText;
        [SerializeField] private TMP_Text _goldText;

        private ConstructionController _subscribed;

        protected override void OnBind()
        {
            if (_upgradeButton != null)
            {
                _upgradeButton.onClick.RemoveListener(OnUpgradeClicked);
                _upgradeButton.onClick.AddListener(OnUpgradeClicked);
            }

            Subscribe(_target);
            SubscribeCurrency(true);
            Refresh();
        }

        public override void OnHide()
        {
            SubscribeCurrency(false);
            Subscribe(null);
            base.OnHide();
        }

        private void SubscribeCurrency(bool subscribe)
        {
            CurrencyManager currency = GameManager.Instance != null ? GameManager.Instance.Currency : null;
            if (currency == null)
            {
                return;
            }

            currency.OnCoinChanged -= OnCoinChanged;
            if (subscribe)
            {
                currency.OnCoinChanged += OnCoinChanged;
            }
        }

        private void OnCoinChanged(double coin)
        {
            if (_target == null || _upgradeButton == null || _target.IsMaxLevel)
            {
                return;
            }

            ConstructionConfig.UpgradeLevel next = _target.Config.Levels[_target.Level];
            _upgradeButton.interactable = GameManager.Instance.Currency.CanAfford(next.Cost);
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
                _levelText.text = $"Level {_target.Level + 1}";
            }

            if (_nameText != null)
            {
                _nameText.text = config.DisplayName;
            }

            if (_secondText != null)
            {
                _secondText.text = $"{config.ProduceTime:0}s";
            }

            if (_goldText != null)
            {
                // Money earned in one cycle = sale payout = (ProfitPerMin / 60) * ProduceTime, after buffs.
                _goldText.text = NumberFormatter.Format(_target.CurrentProfit);
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
