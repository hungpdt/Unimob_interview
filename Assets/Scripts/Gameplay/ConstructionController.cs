using System;
using UnityEngine;

namespace Farm
{
    public class ConstructionController : MonoBehaviour
    {
        [SerializeField] private Transform _harvestPoint;
        [SerializeField] private Transform _deliverySpawn;

        private DeliveryController _delivery;
        private int _level;

        public ConstructionConfig Config { get; private set; }
        public int SlotIndex { get; private set; }
        public int Level { get { return _level; } }
        public int MaxLevel { get { return Config != null ? Config.Levels.Length : 0; } }
        public bool IsMaxLevel { get { return _level >= MaxLevel; } }

        // Falls back to own transform when HarvestPoint is not assigned in the Inspector
        public Transform HarvestPoint { get { return _harvestPoint != null ? _harvestPoint : transform; } }

        public event Action<ConstructionController> OnStatsChanged;

        public void Initialize(ConstructionConfig config, int slotIndex, MarketController market)
        {
            if (config == null)
            {
                Debug.LogError("[ConstructionController] config is null.", this);
                return;
            }

            Config = config;
            SlotIndex = slotIndex;

            GameManager.Instance.Stats.GetProfitStat(slotIndex).BaseValue = config.BaseProfit;
        }

        public double CurrentProfit
        {
            get
            {
                double local = GameManager.Instance.Stats.GetProfitStat(SlotIndex).Value;
                double global = GameManager.Instance.Stats.GetGlobalProfitMultiplier();
                return local * global;
            }
        }

        public bool TryUpgrade()
        {
            if (IsMaxLevel)
            {
                return false;
            }

            double cost = Config.Levels[_level].Cost;
            if (!GameManager.Instance.Currency.TrySpend(cost))
            {
                return false;
            }

            float bonus = Config.Levels[_level].ProfitBonusAdditive;
            GameManager.Instance.Stats.AddConstructionProfitModifier(
                SlotIndex,
                new StatModifier(ModifierType.Additive, bonus, "Level")
            );

            _level++;

            OnStatsChanged?.Invoke(this);
            EventBus.Publish(new ConstructionUpgradedEvent { SlotIndex = SlotIndex, NewLevel = _level });

            return true;
        }

        public double Harvest()
        {
            return CurrentProfit;
        }
    }
}
