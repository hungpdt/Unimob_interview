using UnityEngine;

namespace Farm
{
    public static class EffectSpawner
    {
        public static void Spawn(GameObject prefab, Vector3 pos, float lifetime = 2f)
        {
            if (prefab == null)
            {
                Debug.LogError("[EffectSpawner] prefab is null — assign it in the Inspector.");
                return;
            }

            var go = Object.Instantiate(prefab, pos, Quaternion.identity);
            Object.Destroy(go, lifetime);
        }
    }
}
