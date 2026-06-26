using UnityEngine;

namespace Farm
{
    [CreateAssetMenu(menuName = "Farm/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        public double StartingCoin;
        public ConstructionConfig[] Constructions;  // one entry per slot (2×2 grid), index == slot index
        public UpgradeConfig[] Upgrades;
        public CustomerConfig CustomerConfig;
        public DeliveryConfig DeliveryConfig;
        public int InitialCustomerCount = 1;
    }
}
