using UnityEngine;

namespace Farm
{
    public enum UpgradeEffectType
    {
        GlobalProfitMultiplier,        // x2/x3 all constructions
        ConstructionProfitMultiplier,  // x3 for one construction
        AddCustomer                    // +N customers
    }

    [CreateAssetMenu(menuName = "Farm/UpgradeConfig")]
    public class UpgradeConfig : ScriptableObject
    {
        public string Id;                   // stable key used as StatModifier.Source
        public string Title;
        [TextArea] public string Description;
        public Sprite Icon;
        public UpgradeEffectType EffectType;
        public float Value;                 // 2.0 (x2), 3.0 (x3), or 2 (customers)
        public int TargetSlotIndex = -1;    // only used for ConstructionProfitMultiplier
        public double[] CostTiers;          // escalating costs per purchase
        public bool IsRepeatable;
    }
}
