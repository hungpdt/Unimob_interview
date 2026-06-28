using UnityEngine;
using UnityEngine.AI;

namespace Farm
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class DeliveryController : CharacterBaseController
    {
        [SerializeField] private GameObject _payEffect;

        private StateMachine<DeliveryController> _machine;

        private DeliveryIdleState _idle;
        private MoveToConstructionState _moveToConstruction;
        private HarvestState _harvest;
        private WaitForCustomerState _waitForCustomer;
        private MoveToMarketState _moveToMarket;
        private DeliverState _deliver;
        private DeliveryLeaveState _leave;

        public ConstructionController Owner { get; private set; }
        public MarketController Market { get; private set; }
        public DeliveryConfig Config { get; private set; }

        public double CarriedValue { get; set; }
        public DockController ReservedDock { get; set; }
        public GameObject PayEffect { get { return _payEffect; } }

        public StateMachine<DeliveryController> Machine { get { return _machine; } }
        public DeliveryIdleState IdleState { get { return _idle; } }
        public MoveToConstructionState MoveToConstructionState { get { return _moveToConstruction; } }
        public HarvestState HarvestState { get { return _harvest; } }
        public WaitForCustomerState WaitForCustomerState { get { return _waitForCustomer; } }
        public MoveToMarketState MoveToMarketState { get { return _moveToMarket; } }
        public DeliverState DeliverState { get { return _deliver; } }
        public DeliveryLeaveState LeaveState { get { return _leave; } }

        public void Initialize(ConstructionController owner, MarketController market, DeliveryConfig config)
        {
            if (owner == null)
            {
                Debug.LogError("[DeliveryController] owner is null.", this);
                return;
            }

            if (market == null)
            {
                Debug.LogError("[DeliveryController] market is null.", this);
                return;
            }

            if (config == null)
            {
                Debug.LogError("[DeliveryController] config is null.", this);
                return;
            }

            Owner = owner;
            Market = market;
            Config = config;

            InitCharacter(config.MoveSpeed);

            CarriedValue = 0d;
            ReservedDock = null;

            _machine = new StateMachine<DeliveryController>();
            _idle = new DeliveryIdleState(this, _machine);
            _moveToConstruction = new MoveToConstructionState(this, _machine);
            _harvest = new HarvestState(this, _machine);
            _waitForCustomer = new WaitForCustomerState(this, _machine);
            _moveToMarket = new MoveToMarketState(this, _machine);
            _deliver = new DeliverState(this, _machine);
            _leave = new DeliveryLeaveState(this, _machine);

            _machine.ChangeState(_idle);
        }

        private void Update()
        {
            _machine?.Tick(Time.deltaTime);
        }

        public void Despawn()
        {
            ConstructionController owner = Owner;
            owner?.OnDeliveryFinished();
            GameManager.Instance.Constructions.ReleaseDelivery(this);
        }
    }
}
