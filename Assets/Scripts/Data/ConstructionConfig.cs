using System;
using UnityEngine;

namespace Farm
{
    [CreateAssetMenu(menuName = "Farm/ConstructionConfig")]
    public class ConstructionConfig : ScriptableObject
    {
        public string DisplayName;
        public Sprite Icon;
        public double BaseProfit;       // profit per harvest, before buffs
        public double BuildCost;
        public float ProduceTime;       // seconds per cycle — used to calculate X/min display
        public UpgradeLevel[] Levels;   // index 0 = Lv1 (no bonus)

        [Serializable]
        public struct UpgradeLevel
        {
            public double Cost;
            public float ProfitBonusAdditive;   // 0.10 = +10% at this tier
        }
    }
}
