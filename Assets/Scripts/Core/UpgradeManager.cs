using System.Collections.Generic;
using UnityEngine;

namespace Farm
{
    public class UpgradeManager
    {
        private readonly CurrencyManager _currency;
        private readonly StatService _stats;
        private readonly MarketController _market;
        private readonly GameConfig _config;
        private readonly HashSet<UpgradeConfig> _purchased = new HashSet<UpgradeConfig>();

        public UpgradeManager(CurrencyManager currency, StatService stats, MarketController market, GameConfig config)
        {
            _currency = currency;
            _stats = stats;
            _market = market;
            _config = config;
        }

        public bool IsPurchased(UpgradeConfig upgrade)
        {
            return upgrade != null && _purchased.Contains(upgrade);
        }

        public double GetCost(UpgradeConfig upgrade)
        {
            return upgrade != null ? upgrade.Cost : 0d;
        }

        public bool TryPurchase(UpgradeConfig upgrade)
        {
            if (upgrade == null || IsPurchased(upgrade))
            {
                return false;
            }

            if (!_currency.TrySpend(upgrade.Cost))
            {
                return false;
            }

            if (!ApplyEffect(upgrade))
            {
                _currency.Add(upgrade.Cost);
                return false;
            }

            _purchased.Add(upgrade);

            EventBus.Publish(new UpgradePurchasedEvent { Id = upgrade.name });
            return true;
        }

        private bool ApplyEffect(UpgradeConfig upgrade)
        {
            switch (upgrade.EffectType)
            {
                case UpgradeEffectType.GlobalProfitMultiplier:
                    _stats.AddGlobalProfitModifier(new StatModifier(ModifierType.Multiplicative, upgrade.Value, upgrade));
                    return true;

                case UpgradeEffectType.ConstructionProfitMultiplier:
                    return ApplyConstructionMultiplier(upgrade);

                case UpgradeEffectType.AddCustomer:
                    if (_market == null)
                    {
                        Debug.LogError("[UpgradeManager] _market is null — cannot apply AddCustomer.");
                        return false;
                    }
                    _market.AddCustomers((int)upgrade.Value);
                    return true;
            }

            return false;
        }

        private bool ApplyConstructionMultiplier(UpgradeConfig upgrade)
        {
            if (upgrade.TargetConstruction == null)
            {
                Debug.LogError($"[UpgradeManager] {upgrade.name} has no TargetConstruction.");
                return false;
            }

            if (_config == null || _config.ConstructionsConfigs == null)
            {
                Debug.LogError("[UpgradeManager] _config or its ConstructionsConfigs is null.");
                return false;
            }

            StatModifier mod = new StatModifier(ModifierType.Multiplicative, upgrade.Value, upgrade);

            // Buff every grid slot whose config is this construction type.
            bool applied = false;
            for (int i = 0; i < _config.ConstructionsConfigs.Length; i++)
            {
                if (_config.ConstructionsConfigs[i] == upgrade.TargetConstruction)
                {
                    _stats.AddConstructionProfitModifier(i, mod);
                    applied = true;
                }
            }

            if (!applied)
            {
                Debug.LogError($"[UpgradeManager] {upgrade.name} matched no slot for {upgrade.TargetConstruction.name}.");
            }

            return applied;
        }
    }
}
