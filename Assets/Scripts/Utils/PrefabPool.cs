using System.Collections.Generic;
using UnityEngine;

namespace Farm
{
    public class PrefabPool
    {
        private readonly GameObject _prefab;
        private readonly Queue<GameObject> _idle = new Queue<GameObject>();

        public PrefabPool(GameObject prefab)
        {
            _prefab = prefab;
        }

        public GameObject Get(Vector3 position, Quaternion rotation)
        {
            while (_idle.Count > 0)
            {
                GameObject pooled = _idle.Dequeue();
                if (pooled != null)
                {
                    pooled.transform.SetPositionAndRotation(position, rotation);
                    pooled.SetActive(true);
                    return pooled;
                }
            }

            return Object.Instantiate(_prefab, position, rotation);
        }

        public void Release(GameObject go)
        {
            if (go == null)
            {
                return;
            }

            go.SetActive(false);
            _idle.Enqueue(go);
        }
    }
}
