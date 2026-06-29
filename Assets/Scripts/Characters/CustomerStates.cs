using UnityEngine;

namespace Farm
{
    public class CustomerIdleState : BaseState<CustomerController>
    {
        public CustomerIdleState(CustomerController owner, StateMachine<CustomerController> machine)
            : base(owner, machine) { }

        public override void Enter()
        {
            _owner.StopMovement();
            _owner.SetLocomotion(false, false);
        }

        public override void Tick(float deltaTime) { }
        public override void Exit() { }
    }

    public class MoveToDockState : BaseState<CustomerController>
    {
        public MoveToDockState(CustomerController owner, StateMachine<CustomerController> machine)
            : base(owner, machine) { }

        public override void Enter()
        {
            _owner.SetDestination(_owner.Dock.CustomerAnchor.position);
            _owner.SetLocomotion(true, false);
        }

        public override void Tick(float deltaTime)
        {
            if (_owner.HasArrived(ArriveTolerance))
            {
                _machine.ChangeState(_owner.WaitState);
            }
        }

        public override void Exit() { }
    }

    public class CustomerWaitState : BaseState<CustomerController>
    {
        public CustomerWaitState(CustomerController owner, StateMachine<CustomerController> machine)
            : base(owner, machine) { }

        public override void Enter()
        {
            _owner.StopMovement();
            _owner.IsWaiting = true;
            _owner.SetLocomotion(false, true);
            if (_owner.Dock != null)
            {
                _owner.FaceToward(_owner.Dock.transform.position);
            }
        }

        public override void Tick(float deltaTime) { }

        public override void Exit()
        {
            _owner.IsWaiting = false;
        }
    }

    public class ReceiveState : BaseState<CustomerController>
    {
        private float _elapsed;

        public ReceiveState(CustomerController owner, StateMachine<CustomerController> machine)
            : base(owner, machine) { }

        public override void Enter()
        {
            _elapsed = 0f;
            _owner.SetLocomotion(false, false);
        }

        public override void Tick(float deltaTime)
        {
            _elapsed += deltaTime;
            if (_elapsed >= _owner.Config.ReceiveDuration)
            {
                _machine.ChangeState(_owner.LeaveState);
            }
        }

        public override void Exit() { }
    }

    public class CustomerLeaveState : BaseState<CustomerController>
    {
        public CustomerLeaveState(CustomerController owner, StateMachine<CustomerController> machine)
            : base(owner, machine) { }

        public override void Enter()
        {
            Transform exit = _owner.Market.CustomerExit;
            if (exit == null)
            {
                _owner.DestroyCarriedFruits();
                _owner.Market.ReleaseCustomer(_owner);
                return;
            }

            _owner.SetDestination(exit.position);
            _owner.SetLocomotion(true, false);
        }

        public override void Tick(float deltaTime)
        {
            if (_owner.HasArrived(ArriveTolerance))
            {
                _owner.DestroyCarriedFruits();
                _owner.Market.ReleaseCustomer(_owner);
            }
        }

        public override void Exit() {}
    }
}
