using UnityEngine;

namespace Farm
{
    public class DeliveryIdleState : BaseState<DeliveryController>
    {
        public DeliveryIdleState(DeliveryController owner, StateMachine<DeliveryController> machine)
            : base(owner, machine) { }

        public override void Enter()
        {
            _owner.StopMovement();
            _owner.SetLocomotion(false, false);
        }

        public override void Tick(float deltaTime)
        {
            _machine.ChangeState(_owner.MoveToConstructionState);
        }

        public override void Exit() { }
    }

    public class MoveToConstructionState : BaseState<DeliveryController>
    {
        public MoveToConstructionState(DeliveryController owner, StateMachine<DeliveryController> machine)
            : base(owner, machine) { }

        public override void Enter()
        {
            _owner.SetDestination(_owner.Owner.HarvestPoint.position);
            _owner.SetLocomotion(true, false);
        }

        public override void Tick(float deltaTime)
        {
            if (_owner.HasArrived(ArriveTolerance))
            {
                _machine.ChangeState(_owner.HarvestState);
            }
        }

        public override void Exit() { }
    }

    public class HarvestState : BaseState<DeliveryController>
    {
        private float _elapsed;
        private bool _harvested;

        public HarvestState(DeliveryController owner, StateMachine<DeliveryController> machine)
            : base(owner, machine) { }

        public override void Enter()
        {
            _elapsed = 0f;
            _harvested = false;
            _owner.StopMovement();
            _owner.SetLocomotion(false, false);
            _owner.FaceToward(_owner.Owner.transform.position);
        }

        public override void Tick(float deltaTime)
        {
            if (!_harvested)
            {
                _elapsed += deltaTime;
                if (_elapsed < _owner.Config.HarvestDuration)
                {
                    return;
                }

                _owner.CarriedValue = _owner.Owner.Harvest();
                _owner.AttachFruits(_owner.Owner.TakeFruits());
                EventBus.Publish(new HarvestCompletedEvent
                {
                    SlotIndex = _owner.Owner.SlotIndex,
                    Value = _owner.CarriedValue
                });
                _harvested = true;
            }

            if (!_owner.IsFruitAnimating)
            {
                _owner.Owner.RegrowFruits();
                _machine.ChangeState(_owner.WaitForCustomerState);
            }
        }

        public override void Exit() { }
    }

    public class WaitForCustomerState : BaseState<DeliveryController>
    {
        public WaitForCustomerState(DeliveryController owner, StateMachine<DeliveryController> machine)
            : base(owner, machine) { }

        public override void Enter()
        {
            _owner.StopMovement();
            _owner.SetLocomotion(false, true);
        }

        public override void Tick(float deltaTime)
        {
            DockController dock = _owner.Market.RequestDock();
            if (dock == null)
            {
                return;
            }

            dock.Reserve(_owner);
            _owner.ReservedDock = dock;
            _machine.ChangeState(_owner.MoveToMarketState);
        }

        public override void Exit() { }
    }

    public class MoveToMarketState : BaseState<DeliveryController>
    {
        public MoveToMarketState(DeliveryController owner, StateMachine<DeliveryController> machine)
            : base(owner, machine) { }

        public override void Enter()
        {
            if (_owner.ReservedDock == null || _owner.ReservedDock.DeliveryAnchor == null)
            {
                Debug.LogError("[MoveToMarketState] ReservedDock or DeliveryAnchor is null.", _owner);
                _owner.ReservedDock?.Release();
                _owner.ReservedDock = null;
                _owner.CarriedValue = 0d;
                _machine.ChangeState(_owner.IdleState);
                return;
            }

            _owner.SetDestination(_owner.ReservedDock.DeliveryAnchor.position);
            _owner.SetLocomotion(true, true);
        }

        public override void Tick(float deltaTime)
        {
            if (_owner.HasArrived(ArriveTolerance))
            {
                _machine.ChangeState(_owner.DeliverState);
            }
        }

        public override void Exit() { }
    }

    public class DeliverState : BaseState<DeliveryController>
    {
        private float _elapsed;

        public DeliverState(DeliveryController owner, StateMachine<DeliveryController> machine)
            : base(owner, machine) { }

        public override void Enter()
        {
            _elapsed = 0f;
            _owner.StopMovement();
            _owner.SetLocomotion(false, true);
            if (_owner.ReservedDock != null)
            {
                _owner.FaceToward(_owner.ReservedDock.transform.position);
            }
        }

        public override void Tick(float deltaTime)
        {
            _elapsed += deltaTime;
            if (_elapsed < _owner.Config.DeliverDuration)
            {
                return;
            }

            CompleteSale();
            _machine.ChangeState(_owner.LeaveState);
        }

        private void CompleteSale()
        {
            DockController dock = _owner.ReservedDock;
            Vector3 payPos = dock != null && dock.CurrencyAnchor != null
                ? dock.CurrencyAnchor.position
                : _owner.transform.position;

            _owner.Owner.OnDeliveryFinished();

            GameManager.Instance.Currency.Add(_owner.CarriedValue);

            if (dock != null && dock.Customer != null)
            {
                _owner.SetSaleContext(_owner.CarriedValue, payPos);
                dock.Customer.Receive();
                dock.Customer.AttachFruits(_owner.DetachFruits(), _owner.FruitsDeliveredCallback);
            }
            else
            {
                _owner.DestroyCarriedFruits();
                EffectSpawner.Spawn(_owner.PayEffect, payPos);
                EventBus.Publish(new SaleCompletedEvent { Amount = _owner.CarriedValue, WorldPos = payPos });
            }

            dock?.Release();
            _owner.ClearSaleData();
        }

        public override void Exit() { }
    }

    public class DeliveryLeaveState : BaseState<DeliveryController>
    {
        public DeliveryLeaveState(DeliveryController owner, StateMachine<DeliveryController> machine)
            : base(owner, machine) { }

        public override void Enter()
        {
            Transform exit = _owner.Market.DeliveryExit;
            if (exit == null)
            {
                _owner.Despawn();
                return;
            }

            _owner.SetDestination(exit.position);
            _owner.SetLocomotion(true, false);
        }

        public override void Tick(float deltaTime)
        {
            if (_owner.HasArrived(ArriveTolerance))
            {
                _owner.Despawn();
            }
        }

        public override void Exit() { }
    }
}
