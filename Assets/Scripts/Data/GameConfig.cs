using UnityEngine;

namespace Farm
{
    [CreateAssetMenu(menuName = "Farm/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        public double StartingCoin;
        public ConstructionConfig[] ConstructionsConfigs;  // one entry per slot (2×2 grid), index == slot index
        public UpgradeConfig[] UpgradeConfigs;
        public CustomerConfig CustomerConfig;
        public DeliveryConfig DeliveryConfig;
        public int InitialCustomerCount = 1;
    }
}
