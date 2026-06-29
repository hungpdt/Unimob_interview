using System;
using UnityEngine;

namespace Farm
{
    [CreateAssetMenu(menuName = "Farm/ConstructionConfig")]
    public class ConstructionConfig : ScriptableObject
    {
        public string DisplayName;
        public Sprite Icon;
        public double Yield;            // product units per harvest, before buffs (e.g. 4000 clay)
        public double ProfitPerMin;     // money per minute, before buffs (e.g. 12000 = 12.0k/min)
        public double BuildCost;
        public float ProduceTime;       // seconds per harvest cycle
        public UpgradeLevel[] Levels;   // index 0 = Lv1 (no bonus)

        [Serializable]
        public struct UpgradeLevel
        {
            public double Cost;
            public float ProfitBonusAdditive;   // 0.10 = +10% at this tier
        }
    }
}
