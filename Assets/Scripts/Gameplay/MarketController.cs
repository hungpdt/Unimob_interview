using System.Collections.Generic;
using UnityEngine;

namespace Farm
{
    public class MarketController : MonoBehaviour
    {
        [SerializeField] private DockController[] _docks;
        [SerializeField] private Transform _customerStart;
        [SerializeField] private Transform _customerEnd;
        [SerializeField] private Transform _deliveryEnd;
        [SerializeField] private GameObject _customerPrefab;

        private readonly Queue<CustomerController> _pending = new Queue<CustomerController>();
        private CustomerConfig _config;

        public void Initialize(int startingCustomerCount, CustomerConfig config)
        {
            // TODO: implement
        }

        public void AddCustomers(int count)
        {
            // TODO: implement
        }

        public DockController RequestDock()
        {
            // TODO: implement
            return null;
        }

        public bool HasWaitingCustomer
        {
            get
            {
                // TODO: implement
                return false;
            }
        }
    }
}
