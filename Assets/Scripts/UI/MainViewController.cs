using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Farm
{
    public class MainViewController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _coinText;
        [SerializeField] private Button _openUpgradeButton;
        [SerializeField] private Button _cheatMoneyButton;

        private CurrencyManager _currency;

        public void Bind(CurrencyManager currency)
        {
            if (currency == null)
            {
                Debug.LogError("[MainViewController] currency is null.", this);
                return;
            }

            _currency = currency;
            _currency.OnCoinChanged += OnCoinChanged;

            if (_openUpgradeButton != null)
            {
                _openUpgradeButton.onClick.AddListener(OpenUpgradePanel);
            }

            if (_cheatMoneyButton != null)
            {
                _cheatMoneyButton.onClick.AddListener(CheatMoney);
            }

            OnCoinChanged(_currency.Coin);
        }

        private void OnDestroy()
        {
            if (_currency != null)
            {
                _currency.OnCoinChanged -= OnCoinChanged;
            }
        }

        private void CheatMoney()
        {
            _currency.Add(GameManager.Instance.Config.CheatCoinAmount);
        }

        private void OnCoinChanged(double coin)
        {
            if (_coinText != null)
            {
                _coinText.text = NumberFormatter.Format(coin);
            }
        }

        private void OpenUpgradePanel()
        {
            UpgradeViewController view = UIManager.Instance.Show<UpgradeViewController>();
            if (view == null)
            {
                return;
            }

            view.Bind(GameManager.Instance.Config.UpgradeConfigs, GameManager.Instance.Upgrades);
        }
    }
}
