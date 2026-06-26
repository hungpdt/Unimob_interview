namespace Farm
{
    public class CustomerIdleState : BaseState<CustomerController>
    {
        public CustomerIdleState(CustomerController owner, StateMachine<CustomerController> machine)
            : base(owner, machine) { }

        public override void Enter() { /* TODO */ }
        public override void Tick(float deltaTime) { /* TODO */ }
        public override void Exit() { /* TODO */ }
    }

    public class MoveToDockState : BaseState<CustomerController>
    {
        public MoveToDockState(CustomerController owner, StateMachine<CustomerController> machine)
            : base(owner, machine) { }

        public override void Enter() { /* TODO */ }
        public override void Tick(float deltaTime) { /* TODO */ }
        public override void Exit() { /* TODO */ }
    }

    public class CustomerWaitState : BaseState<CustomerController>
    {
        public CustomerWaitState(CustomerController owner, StateMachine<CustomerController> machine)
            : base(owner, machine) { }

        public override void Enter() { /* TODO */ }
        public override void Tick(float deltaTime) { /* TODO */ }
        public override void Exit() { /* TODO */ }
    }

    public class ReceiveState : BaseState<CustomerController>
    {
        public ReceiveState(CustomerController owner, StateMachine<CustomerController> machine)
            : base(owner, machine) { }

        public override void Enter() { /* TODO */ }
        public override void Tick(float deltaTime) { /* TODO */ }
        public override void Exit() { /* TODO */ }
    }

    public class CustomerLeaveState : BaseState<CustomerController>
    {
        public CustomerLeaveState(CustomerController owner, StateMachine<CustomerController> machine)
            : base(owner, machine) { }

        public override void Enter() { /* TODO */ }
        public override void Tick(float deltaTime) { /* TODO */ }
        public override void Exit() { /* TODO */ }
    }
}
