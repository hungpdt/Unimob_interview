using UnityEngine;

namespace Farm
{
    [DefaultExecutionOrder(-100)]
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;

        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("[GameManager] Instance is null — GameManager is missing from the scene.");
                }
                return _instance;
            }
            private set { _instance = value; }
        }

        [SerializeField] private GameConfig _gameConfig;
        [SerializeField] private MainViewController _mainView;

        public CurrencyManager Currency { get; private set; }
        public StatService Stats { get; private set; }
        public UpgradeManager Upgrades { get; private set; }
        public ConstructionManager Constructions { get; private set; }
        public MarketController Market { get; private set; }

        public GameConfig Config { get { return _gameConfig; } }

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            if (_gameConfig == null)
            {
                Debug.LogError("[GameManager] _gameConfig is null — assign GameConfig SO in the Inspector.", this);
                return;
            }

            _instance = this;

            Currency      = new CurrencyManager(_gameConfig.StartingCoin);
            Stats         = new StatService();
            Constructions = GetComponentInChildren<ConstructionManager>();
            Market        = GetComponentInChildren<MarketController>();
            Upgrades      = new UpgradeManager(Currency, Stats);

            if (Constructions == null)
            {
                Debug.LogError("[GameManager] ConstructionManager not found in children.", this);
                return;
            }

            Constructions.Initialize(_gameConfig);
        }

        private void Start()
        {
            if (_mainView == null)
            {
                Debug.LogError("[GameManager] _mainView is null — assign MainViewController in the Inspector.", this);
                return;
            }

            _mainView.gameObject.SetActive(true);
            _mainView.Bind(Currency);

            EventBus.Publish(new GameReadyEvent());
        }

        private void OnDestroy()
        {
            EventBus.Clear();
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}
