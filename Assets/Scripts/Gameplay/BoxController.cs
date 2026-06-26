using System;
using UnityEngine;

namespace Farm
{
    public class BoxController : MonoBehaviour
    {
        [SerializeField] private GameObject _constructionPrefab;
        [SerializeField] private GameObject _effBuildDone;

        public ConstructionConfig Config { get; private set; }
        public int SlotIndex { get; private set; }

        public event Action<BoxController> OnBuildConfirmed;

        public void Initialize(ConstructionConfig config, int slotIndex)
        {
            if (config == null)
            {
                Debug.LogError($"[BoxController] ConstructionConfig for slot {slotIndex} is null.", this);
                return;
            }

            Config = config;
            SlotIndex = slotIndex;
        }

        private void OnMouseDown()
        {
            if (Config == null)
            {
                return;
            }

            ConstructionBuildViewController view = UIManager.Instance.Show<ConstructionBuildViewController>();
            if (view != null)
            {
                view.Bind(this);
            }
        }

        public void Build()
        {
            if (_constructionPrefab == null)
            {
                Debug.LogError("[BoxController] _constructionPrefab is null — assign it in the Inspector.", this);
                return;
            }

            if (!GameManager.Instance.Currency.TrySpend(Config.BuildCost))
            {
                return;
            }

            GameObject go = Instantiate(_constructionPrefab, transform.position, Quaternion.identity);
            ConstructionController construction = go.GetComponent<ConstructionController>();

            if (construction == null || _effBuildDone == null)
            {
                Debug.LogError("[BoxController] _constructionPrefab does not have a ConstructionController component.", this);
                Destroy(go);
                return;
            }

            construction.Initialize(Config, SlotIndex, GameManager.Instance.Market);
            GameManager.Instance.Constructions.Register(construction);

            EffectSpawner.Spawn(_effBuildDone, transform.position);
            EventBus.Publish(new ConstructionBuiltEvent { SlotIndex = SlotIndex });

            OnBuildConfirmed?.Invoke(this);
            gameObject.SetActive(false);
        }
    }
}
