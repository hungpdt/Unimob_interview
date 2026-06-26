using UnityEngine;

namespace Farm
{
    public class DockController : MonoBehaviour
    {
        [SerializeField] private Transform _deliveryAnchor;   // delivery person stands here
        [SerializeField] private Transform _customerAnchor;   // customer stands here
        [SerializeField] private Transform _currencyAnchor;   // payment effect spawn point

        public Transform DeliveryAnchor { get { return _deliveryAnchor; } }
        public Transform CustomerAnchor { get { return _customerAnchor; } }
        public Transform CurrencyAnchor { get { return _currencyAnchor; } }

        public CustomerController Customer { get; private set; }
        public bool IsOccupied { get { return Customer != null; } }
        public bool IsReserved { get; private set; }

        public void Occupy(CustomerController customer)
        {
            // TODO: implement
        }

        public void Reserve(DeliveryController delivery)
        {
            // TODO: implement
        }

        public void Release()
        {
            // TODO: implement
        }
    }
}
