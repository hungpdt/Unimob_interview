using UnityEngine;

namespace Farm
{
    public class DockController : MonoBehaviour
    {
        [SerializeField] private Transform _deliveryAnchor;
        [SerializeField] private Transform _customerAnchor;
        [SerializeField] private Transform _currencyAnchor;

        public Transform DeliveryAnchor { get { return _deliveryAnchor; } }
        public Transform CustomerAnchor { get { return _customerAnchor; } }
        public Transform CurrencyAnchor { get { return _currencyAnchor; } }

        public CustomerController Customer { get; private set; }
        public DeliveryController ReservedBy { get; private set; }
        public bool IsOccupied { get { return Customer != null; } }
        public bool IsReserved { get; private set; }

        public void Occupy(CustomerController customer)
        {
            Customer = customer;
        }

        public void Reserve(DeliveryController delivery)
        {
            IsReserved = true;
            ReservedBy = delivery;
        }

        public void Release()
        {
            IsReserved = false;
            ReservedBy = null;
            Customer = null;
        }
    }
}
