using UnityEngine;
using UnityEngine.AI;

namespace Farm
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class CustomerController : MonoBehaviour
    {
        private StateMachine<CustomerController> _machine;
        private NavMeshAgent _agent;
        private Animator _animator;

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
            // TODO: implement
        }

        private void Update()
        {
            _machine?.Tick(Time.deltaTime);
        }

        public void AssignDock(DockController dock)
        {
            // TODO: implement
        }

        public void Receive()
        {
            // TODO: implement
        }

        public void SetDestination(Vector3 pos)
        {
            // TODO: implement
        }

        public bool HasArrived(float tolerance)
        {
            // TODO: implement
            return false;
        }
    }
}
