using System.Collections.Generic;
using UnityEngine;

namespace Farm
{
    public class ConstructionManager : MonoBehaviour
    {
        [SerializeField] private ConstructionController[] _plots;
        [SerializeField] private GameObject _deliveryPrefab;
        [SerializeField] private Transform _deliverySpawn;

        private readonly List<ConstructionController> _constructions = new List<ConstructionController>();
        private PrefabPool _deliveryPool;

        public IReadOnlyList<ConstructionController> Constructions { get { return _constructions; } }

        public void Initialize(GameConfig config)
        {
            if (config == null)
            {
                Debug.LogError("[ConstructionManager] GameConfig is null.", this);
                return;
            }

            if (_plots == null || _plots.Length == 0)
            {
                Debug.LogError("[ConstructionManager] _plots is not assigned in the Inspector.", this);
                return;
            }

            int count = Mathf.Min(_plots.Length, config.ConstructionsConfigs.Length);
            for (int i = 0; i < count; i++)
            {
                if (_plots[i] == null)
                {
                    Debug.LogError($"[ConstructionManager] _plots[{i}] is null.", this);
                    continue;
                }

                _plots[i].Initialize(config.ConstructionsConfigs[i], i);
            }
        }

        public void Register(ConstructionController construction)
        {
            if (construction == null)
            {
                Debug.LogError("[ConstructionManager] Tried to register a null ConstructionController.", this);
                return;
            }

            _constructions.Add(construction);
        }

        public DeliveryController SpawnDelivery(ConstructionController owner)
        {
            if (owner == null)
            {
                Debug.LogError("[ConstructionManager] SpawnDelivery owner is null.", this);
                return null;
            }

            if (_deliveryPrefab == null)
            {
                Debug.LogError("[ConstructionManager] _deliveryPrefab is null.", this);
                return null;
            }

            MarketController market = GameManager.Instance.Market;
            if (market == null)
            {
                Debug.LogError("[ConstructionManager] GameManager.Market is null.", this);
                return null;
            }

            if (_deliveryPool == null)
            {
                _deliveryPool = new PrefabPool(_deliveryPrefab);
            }

            Transform spawn = _deliverySpawn != null ? _deliverySpawn : transform;
            GameObject go = _deliveryPool.Get(spawn.position, spawn.rotation);

            DeliveryController delivery = go.GetComponent<DeliveryController>();
            if (delivery == null)
            {
                Debug.LogError("[ConstructionManager] _deliveryPrefab has no DeliveryController.", this);
                Destroy(go);
                return null;
            }

            delivery.Initialize(owner, market, GameManager.Instance.Config.DeliveryConfig);
            return delivery;
        }

        public void ReleaseDelivery(DeliveryController delivery)
        {
            if (delivery == null)
            {
                return;
            }

            if (_deliveryPool == null)
            {
                Destroy(delivery.gameObject);
                return;
            }

            _deliveryPool.Release(delivery.gameObject);
        }

        public ConstructionController GetBySlot(int slotIndex)
        {
            foreach (ConstructionController c in _constructions)
            {
                if (c.SlotIndex == slotIndex)
                {
                    return c;
                }
            }
            return null;
        }
    }
}
