using System.Collections.Generic;
using UnityEngine;

namespace Farm
{
    public class MarketController : MonoBehaviour
    {
        [SerializeField] private DockController[] _docks;
        [SerializeField] private Transform _customerStart;
        [SerializeField] private GameObject _customerPrefab;

        [field: SerializeField] public Transform CustomerExit { get; private set; }
        [field: SerializeField] public Transform DeliveryExit { get; private set; }

        private readonly Queue<CustomerController> _pending = new Queue<CustomerController>();
        private readonly List<CustomerController> _customers = new List<CustomerController>();
        private CustomerConfig _config;
        private int _targetCount;
        private PrefabPool _customerPool;

        public void Initialize(int startingCustomerCount, CustomerConfig config)
        {
            if (config == null)
            {
                Debug.LogError("[MarketController] config is null.", this);
                return;
            }

            if (_customerPrefab == null)
            {
                Debug.LogError("[MarketController] _customerPrefab is null.", this);
                return;
            }

            if (_docks == null || _docks.Length == 0)
            {
                Debug.LogError("[MarketController] _docks is not assigned in the Inspector.", this);
                return;
            }

            if (_customerStart == null)
            {
                Debug.LogError("[MarketController] _customerStart is null.", this);
                return;
            }

            _config = config;
            AddCustomers(startingCustomerCount);
        }

        public void AddCustomers(int count)
        {
            if (count <= 0)
            {
                return;
            }

            _targetCount += count;
            for (int i = 0; i < count; i++)
            {
                SpawnCustomer();
            }
        }

        public DockController RequestDock()
        {
            for (int i = 0; i < _docks.Length; i++)
            {
                DockController dock = _docks[i];
                if (dock != null && dock.IsOccupied && !dock.IsReserved && dock.Customer.IsWaiting)
                {
                    return dock;
                }
            }

            return null;
        }

        private void Update()
        {
            if (_config == null)
            {
                return;
            }

            BackfillCustomers();
            AssignPendingToDocks();
        }

        private void BackfillCustomers()
        {
            for (int i = _customers.Count - 1; i >= 0; i--)
            {
                if (_customers[i] == null)
                {
                    _customers.RemoveAt(i);
                }
            }

            while (_customers.Count < _targetCount)
            {
                if (!SpawnCustomer())
                {
                    break;
                }
            }
        }

        private void AssignPendingToDocks()
        {
            while (_pending.Count > 0)
            {
                DockController dock = FindFreeDock();
                if (dock == null)
                {
                    return;
                }

                CustomerController customer = _pending.Dequeue();
                if (customer == null)
                {
                    continue;
                }

                dock.Occupy(customer);
                customer.AssignDock(dock);
            }
        }

        private DockController FindFreeDock()
        {
            for (int i = 0; i < _docks.Length; i++)
            {
                DockController dock = _docks[i];
                if (dock != null && !dock.IsOccupied && !dock.IsReserved)
                {
                    return dock;
                }
            }

            return null;
        }

        private bool SpawnCustomer()
        {
            if (_customerPool == null)
            {
                _customerPool = new PrefabPool(_customerPrefab);
            }

            GameObject go = _customerPool.Get(_customerStart.position, _customerStart.rotation);
            CustomerController customer = go.GetComponent<CustomerController>();
            if (customer == null)
            {
                Debug.LogError("[MarketController] _customerPrefab has no CustomerController.", this);
                Destroy(go);
                return false;
            }

            customer.Initialize(this, _config);
            _customers.Add(customer);
            _pending.Enqueue(customer);
            return true;
        }

        public void ReleaseCustomer(CustomerController customer)
        {
            if (customer == null)
            {
                return;
            }

            _customers.Remove(customer);

            if (_customerPool == null)
            {
                Destroy(customer.gameObject);
                return;
            }

            _customerPool.Release(customer.gameObject);
        }
    }
}
