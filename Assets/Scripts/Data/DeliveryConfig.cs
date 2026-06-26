using UnityEngine;

namespace Farm
{
    [CreateAssetMenu(menuName = "Farm/DeliveryConfig")]
    public class DeliveryConfig : ScriptableObject
    {
        public float MoveSpeed;
        public float HarvestDuration;
        public float DeliverDuration;
        public int CarryCapacity;   // produce units carried per trip
    }
}
