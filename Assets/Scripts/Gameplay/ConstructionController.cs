using System;
using System.Collections;
using UnityEngine;

namespace Farm
{
    public class ConstructionController : MonoBehaviour
    {
        [SerializeField] private Transform _harvestPoint;

        [SerializeField] private GameObject _boxState;
        [SerializeField] private GameObject _builtState;
        [SerializeField] private GameObject[] _fruits;
        [SerializeField] private float _fruitRevealInterval = 0.15f;
        [SerializeField] private BuildTimerView _timerView;
        [SerializeField] private ConstructionInfoView _infoView;
        [SerializeField] private GameObject _effBuildDone;
        [SerializeField] private float _buildDuration = 1f;
        
        private Animation _boxAnimation;
        private const string OpenClip = "BoxOpen";

        private DeliveryController _delivery;
        private int _level;
        private StatCollection _profitStat;
        private StatCollection _globalProfitStat;
        private bool _isBuilt;
        private bool _isBuilding;
        private Coroutine _revealRoutine;

        public ConstructionConfig Config { get; private set; }
        public int SlotIndex { get; private set; }
        public int Level { get { return _level; } }
        public int MaxLevel { get { return Config != null ? Config.Levels.Length : 0; } }
        public bool IsMaxLevel { get { return _level >= MaxLevel; } }
        public bool IsBuilt { get { return _isBuilt; } }

        public Transform HarvestPoint { get { return _harvestPoint != null ? _harvestPoint : transform; } }

        public event Action<ConstructionController> OnStatsChanged;

        public void Initialize(ConstructionConfig config, int slotIndex)
        {
            if (config == null)
            {
                Debug.LogError("[ConstructionController] config is null.", this);
                return;
            }

            Config = config;
            SlotIndex = slotIndex;

            if (_boxAnimation == null && _boxState != null)
            {
                _boxAnimation = _boxState.GetComponentInChildren<Animation>(true);
            }

            ShowUnbuilt();
        }

        private void OnMouseDown()
        {
            if (Config == null || _isBuilding)
            {
                return;
            }

            if (_isBuilt)
            {
                if (UIManager.Instance.IsOpen<ConstructionUpgradeViewController>())
                {
                    return;
                }

                ConstructionUpgradeViewController upgradeView = UIManager.Instance.Show<ConstructionUpgradeViewController>();
                if (upgradeView != null)
                {
                    upgradeView.Bind(this);
                }
                return;
            }

            if (UIManager.Instance.IsOpen<ConstructionBuildViewController>())
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
            if (_isBuilt || _isBuilding)
            {
                return;
            }

            if (Config == null)
            {
                Debug.LogError("[ConstructionController] Build called before Initialize — Config is null.", this);
                return;
            }

            if (!GameManager.Instance.Currency.TrySpend(Config.BuildCost))
            {
                return;
            }

            _isBuilding = true;
            StartCoroutine(BuildSequence());
        }

        private IEnumerator BuildSequence()
        {
            if (_timerView != null)
            {
                _timerView.gameObject.SetActive(true);
            }

            if (_boxAnimation != null)
            {
                _boxAnimation.Play(OpenClip);
            }

            float duration = _buildDuration > 0f ? _buildDuration : 1f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float remaining = Mathf.Max(0f, duration - elapsed);
                if (_timerView != null)
                {
                    _timerView.SetProgress(remaining / duration, remaining);
                }
                yield return null;
            }

            if (_timerView != null)
            {
                _timerView.gameObject.SetActive(false);
            }

            CompleteBuild();
        }

        private void CompleteBuild()
        {
            ShowBuilt();

            if (_effBuildDone != null)
            {
                EffectSpawner.Spawn(_effBuildDone, transform.position);
            }

            StatService stats = GameManager.Instance.Stats;
            _profitStat = stats.GetProfitStat(SlotIndex);
            _profitStat.BaseValue = Config.BaseProfit;
            _globalProfitStat = stats.GlobalProfit;

            _profitStat.OnChanged += HandleStatsChanged;
            _globalProfitStat.OnChanged += HandleStatsChanged;
            GameManager.Instance.Constructions.Register(this);

            _isBuilding = false;
            _isBuilt = true;

            if (_infoView != null)
            {
                _infoView.gameObject.SetActive(true);
                _infoView.Bind(this);
            }

            SpawnDelivery();

            EventBus.Publish(new ConstructionBuiltEvent { SlotIndex = SlotIndex });
        }

        private void SpawnDelivery()
        {
            if (_delivery != null)
            {
                return;
            }

            _delivery = GameManager.Instance.Constructions.SpawnDelivery(this);
        }

        public void OnDeliveryFinished()
        {
            _delivery = null;
            SpawnDelivery();
        }

        public void HandleStatsChanged()
        {
            OnStatsChanged?.Invoke(this);
        }

        private void ShowUnbuilt()
        {
            if (_boxState != null)
            {
                _boxState.SetActive(true);
            }

            if (_builtState != null)
            {
                _builtState.SetActive(false);
            }

            if (_timerView != null)
            {
                _timerView.gameObject.SetActive(false);
            }

            if (_infoView != null)
            {
                _infoView.gameObject.SetActive(false);
            }

            if (_revealRoutine != null)
            {
                StopCoroutine(_revealRoutine);
                _revealRoutine = null;
            }

            SetFruitsActive(false);
        }

        private void ShowBuilt()
        {
            if (_boxState != null)
            {
                _boxState.SetActive(false);
            }

            if (_builtState != null)
            {
                _builtState.SetActive(true);
            }

            if (_revealRoutine != null)
            {
                StopCoroutine(_revealRoutine);
            }

            _revealRoutine = StartCoroutine(RevealFruitsRoutine());
        }

        private IEnumerator RevealFruitsRoutine()
        {
            if (_fruits != null)
            {
                foreach (GameObject fruit in _fruits)
                {
                    if (fruit != null)
                    {
                        fruit.SetActive(true);
                    }

                    if (_fruitRevealInterval > 0f)
                    {
                        yield return new WaitForSeconds(_fruitRevealInterval);
                    }
                }
            }

            _revealRoutine = null;
        }

        private void SetFruitsActive(bool active)
        {
            if (_fruits == null)
            {
                return;
            }

            foreach (GameObject fruit in _fruits)
            {
                if (fruit != null)
                {
                    fruit.SetActive(active);
                }
            }
        }

        public double CurrentProfit
        {
            get
            {
                double local = GameManager.Instance.Stats.GetProfitStat(SlotIndex).Value;
                double global = GameManager.Instance.Stats.GetGlobalProfitMultiplier();
                return local * global;
            }
        }

        public bool TryUpgrade()
        {
            if (IsMaxLevel)
            {
                return false;
            }

            double cost = Config.Levels[_level].Cost;
            float bonus = Config.Levels[_level].ProfitBonusAdditive;
            if (!GameManager.Instance.Currency.TrySpend(cost))
            {
                return false;
            }

            _level++;
            GameManager.Instance.Stats.AddConstructionProfitModifier(
                SlotIndex,
                new StatModifier(ModifierType.Additive, bonus, "Level")
            );

            EventBus.Publish(new ConstructionUpgradedEvent { SlotIndex = SlotIndex, NewLevel = _level });

            return true;
        }

        public double Harvest()
        {
            return CurrentProfit;
        }

        private void OnDestroy()
        {
            if (_profitStat != null)
            {
                _profitStat.OnChanged -= HandleStatsChanged;
            }

            if (_globalProfitStat != null)
            {
                _globalProfitStat.OnChanged -= HandleStatsChanged;
            }
        }
    }
}
