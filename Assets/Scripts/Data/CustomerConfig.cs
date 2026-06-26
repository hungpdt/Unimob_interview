using UnityEngine;

namespace Farm
{
    [CreateAssetMenu(menuName = "Farm/CustomerConfig")]
    public class CustomerConfig : ScriptableObject
    {
        public float MoveSpeed;
        public float WaitTimeout;   // optional: leave if not served within this time
    }
}
