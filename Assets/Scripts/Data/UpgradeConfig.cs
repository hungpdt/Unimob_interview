using UnityEngine;

namespace Farm
{
    public enum UpgradeEffectType
    {
        GlobalProfitMultiplier,
        ConstructionProfitMultiplier,
        AddCustomer
    }

    [CreateAssetMenu(menuName = "Farm/UpgradeConfig")]
    public class UpgradeConfig : ScriptableObject
    {
        public string Title;
        [TextArea] public string Description;
        public Sprite Icon;
        public double Cost;
        public UpgradeEffectType EffectType;
        public float Value;
        public ConstructionConfig TargetConstruction;  // only for ConstructionProfitMultiplier
    }
}
