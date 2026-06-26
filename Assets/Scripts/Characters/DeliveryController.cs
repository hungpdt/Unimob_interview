using UnityEngine;
using UnityEngine.AI;

namespace Farm
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class DeliveryController : MonoBehaviour
    {
        private StateMachine<DeliveryController> _machine;
        private NavMeshAgent _agent;
        private Animator _animator;

        // State instances held as fields to avoid per-transition allocation.
        private DeliveryIdleState _idle;
        private MoveToConstructionState _moveToConstruction;
        private HarvestState _harvest;
        private MoveToMarketState _moveToMarket;
        private DeliverState _deliver;

        public ConstructionController Owner { get; private set; }
        public MarketController Market { get; private set; }
        public DeliveryConfig Config { get; private set; }

        public double CarriedValue { get; set; }   // value locked in at harvest time

        public StateMachine<DeliveryController> Machine { get { return _machine; } }
        public DeliveryIdleState IdleState { get { return _idle; } }
        public MoveToConstructionState MoveToConstructionState { get { return _moveToConstruction; } }
        public HarvestState HarvestState { get { return _harvest; } }
        public MoveToMarketState MoveToMarketState { get { return _moveToMarket; } }
        public DeliverState DeliverState { get { return _deliver; } }

        public void Initialize(ConstructionController owner, MarketController market, DeliveryConfig config)
        {
            // TODO: implement
        }

        private void Update()
        {
            _machine?.Tick(Time.deltaTime);
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

        public void PlayAnim(string trigger)
        {
            // TODO: implement
        }
    }
}
