using System.Collections.Generic;
using UnityEngine;

namespace Farm
{
    public class ConstructionManager : MonoBehaviour
    {
        [SerializeField] private ConstructionController[] _plots;

        private readonly List<ConstructionController> _constructions = new List<ConstructionController>();

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

            int count = Mathf.Min(_plots.Length, config.Constructions.Length);
            for (int i = 0; i < count; i++)
            {
                if (_plots[i] == null)
                {
                    Debug.LogError($"[ConstructionManager] _plots[{i}] is null.", this);
                    continue;
                }

                _plots[i].Initialize(config.Constructions[i], i);
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
