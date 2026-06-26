using System;

namespace Farm
{
    public class CurrencyManager
    {
        public event Action<double> OnCoinChanged;

        private double _coin;

        public double Coin { get { return _coin; } }

        public CurrencyManager(double startingCoin)
        {
            _coin = Math.Max(0d, startingCoin);
        }

        public void Add(double amount)
        {
            _coin = Math.Max(0d, _coin + amount);
            OnCoinChanged?.Invoke(_coin);
        }

        public bool CanAfford(double cost)
        {
            return _coin >= cost;
        }

        public bool TrySpend(double cost)
        {
            if (!CanAfford(cost))
            {
                return false;
            }

            _coin -= cost;
            OnCoinChanged?.Invoke(_coin);
            return true;
        }

        public void SetCoin(double value)
        {
            _coin = Math.Max(0d, value);
            OnCoinChanged?.Invoke(_coin);
        }
    }
}
