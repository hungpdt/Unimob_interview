using System.Collections.Generic;

namespace Farm
{
    public class UpgradeManager
    {
        private readonly CurrencyManager _currency;
        private readonly StatService _stats;
        private readonly Dictionary<string, int> _counts = new Dictionary<string, int>();

        public UpgradeManager(CurrencyManager currency, StatService stats)
        {
            _currency = currency;
            _stats = stats;
        }

        public bool TryPurchase(UpgradeConfig upgrade)
        {
            // TODO: implement
            return false;
        }

        public double GetCurrentCost(UpgradeConfig upgrade)
        {
                // TODO: implement
            return 0d;
        }

        public int GetPurchaseCount(UpgradeConfig upgrade)
        {
            // TODO: implement
            return 0;
        }
    }
}
