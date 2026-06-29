using UnityEngine;

namespace Farm
{
    [CreateAssetMenu(menuName = "Farm/CustomerConfig")]
    public class CustomerConfig : ScriptableObject
    {
        public float MoveSpeed;
        public float ReceiveDuration = 1.0f;
    }
}
