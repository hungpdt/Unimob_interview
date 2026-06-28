using UnityEngine;
using UnityEngine.AI;

namespace Farm
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class CustomerController : CharacterBaseController
    {
        private StateMachine<CustomerController> _machine;

        private CustomerIdleState _idle;
        private MoveToDockState _moveToDock;
        private CustomerWaitState _wait;
        private ReceiveState _receive;
        private CustomerLeaveState _leave;

        public MarketController Market { get; private set; }
        public CustomerConfig Config { get; private set; }
        public DockController Dock { get; private set; }

        public bool IsWaiting { get; set; }

        public StateMachine<CustomerController> Machine { get { return _machine; } }
        public CustomerIdleState IdleState { get { return _idle; } }
        public MoveToDockState MoveToDockState { get { return _moveToDock; } }
        public CustomerWaitState WaitState { get { return _wait; } }
        public ReceiveState ReceiveState { get { return _receive; } }
        public CustomerLeaveState LeaveState { get { return _leave; } }

        public void Initialize(MarketController market, CustomerConfig config)
        {
            if (market == null)
            {
                Debug.LogError("[CustomerController] market is null.", this);
                return;
            }

            if (config == null)
            {
                Debug.LogError("[CustomerController] config is null.", this);
                return;
            }

            Market = market;
            Config = config;

            InitCharacter(config.MoveSpeed);

            Dock = null;
            IsWaiting = false;

            _machine = new StateMachine<CustomerController>();
            _idle = new CustomerIdleState(this, _machine);
            _moveToDock = new MoveToDockState(this, _machine);
            _wait = new CustomerWaitState(this, _machine);
            _receive = new ReceiveState(this, _machine);
            _leave = new CustomerLeaveState(this, _machine);

            _machine.ChangeState(_idle);
        }

        private void Update()
        {
            _machine?.Tick(Time.deltaTime);
        }

        public void AssignDock(DockController dock)
        {
            Dock = dock;
            _machine.ChangeState(_moveToDock);
        }

        public void Receive()
        {
            _machine.ChangeState(_receive);
        }
    }
}
