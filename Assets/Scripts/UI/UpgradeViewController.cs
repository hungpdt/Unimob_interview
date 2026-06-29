using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Farm
{
    public class UpgradeViewController : BaseViewController
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private RectTransform _content;
        [SerializeField] private GameObject _itemPrefab;

        private readonly List<UpgradeItemView> _items = new List<UpgradeItemView>();
        private IReadOnlyList<UpgradeConfig> _configs;
        private UpgradeManager _upgrades;
        private CurrencyManager _currency;

        public void Bind(IReadOnlyList<UpgradeConfig> configs, UpgradeManager upgrades)
        {
            if (configs == null || upgrades == null)
            {
                Debug.LogError("[UpgradeViewController] configs or upgrades is null.", this);
                return;
            }

            if (_content == null || _itemPrefab == null)
            {
                Debug.LogError("[UpgradeViewController] _content or _itemPrefab is null.", this);
                return;
            }

            _configs = configs;
            _upgrades = upgrades;

            BuildItems();
            SubscribeCurrency(true);
            RefreshAll();
        }

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

            SubscribeCurrency(false);
            base.OnHide();
        }

        private void OnCloseClicked()
        {
            UIManager.Instance.HideTop();
        }

        private void BuildItems()
        {
            // Reuse existing instances across re-binds; only create the shortfall.
            for (int i = _items.Count; i < _configs.Count; i++)
            {
                GameObject go = Instantiate(_itemPrefab, _content, false);
                UpgradeItemView item = go.GetComponent<UpgradeItemView>();
                if (item == null)
                {
                    Debug.LogError("[UpgradeViewController] _itemPrefab has no UpgradeItemView.", this);
                    Destroy(go);
                    return;
                }
                _items.Add(item);
            }

            for (int i = 0; i < _items.Count; i++)
            {
                bool used = i < _configs.Count;
                _items[i].gameObject.SetActive(used);
                if (!used)
                {
                    continue;
                }

                UpgradeConfig config = _configs[i];
                _items[i].Bind(config, _upgrades.GetCost(config), () => OnPurchase(config));
            }
        }

        private void OnPurchase(UpgradeConfig config)
        {
            if (_upgrades != null && _upgrades.TryPurchase(config))
            {
                RefreshAll();
            }
        }

        private void RefreshAll()
        {
            if (_upgrades == null || _configs == null)
            {
                return;
            }

            for (int i = 0; i < _items.Count && i < _configs.Count; i++)
            {
                UpgradeConfig config = _configs[i];

                if (_upgrades.IsPurchased(config))
                {
                    _items[i].gameObject.SetActive(false);
                    continue;
                }

                double cost = _upgrades.GetCost(config);
                bool canAfford = _currency != null && _currency.CanAfford(cost);
                _items[i].Refresh(cost, canAfford);
            }
        }

        private void SubscribeCurrency(bool subscribe)
        {
            if (_currency != null)
            {
                _currency.OnCoinChanged -= OnCoinChanged;
                _currency = null;
            }

            if (!subscribe || GameManager.Instance == null)
            {
                return;
            }

            _currency = GameManager.Instance.Currency;
            if (_currency != null)
            {
                _currency.OnCoinChanged += OnCoinChanged;
            }
        }

        private void OnCoinChanged(double coin)
        {
            RefreshAll();
        }
    }
}
