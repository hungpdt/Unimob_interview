namespace Farm
{
    public class DeliveryIdleState : BaseState<DeliveryController>
    {
        public DeliveryIdleState(DeliveryController owner, StateMachine<DeliveryController> machine)
            : base(owner, machine) { }

        public override void Enter() { /* TODO */ }
        public override void Tick(float deltaTime) { /* TODO */ }
        public override void Exit() { /* TODO */ }
    }

    public class MoveToConstructionState : BaseState<DeliveryController>
    {
        public MoveToConstructionState(DeliveryController owner, StateMachine<DeliveryController> machine)
            : base(owner, machine) { }

        public override void Enter() { /* TODO */ }
        public override void Tick(float deltaTime) { /* TODO */ }
        public override void Exit() { /* TODO */ }
    }

    public class HarvestState : BaseState<DeliveryController>
    {
        public HarvestState(DeliveryController owner, StateMachine<DeliveryController> machine)
            : base(owner, machine) { }

        public override void Enter() { /* TODO */ }
        public override void Tick(float deltaTime) { /* TODO */ }
        public override void Exit() { /* TODO */ }
    }

    public class MoveToMarketState : BaseState<DeliveryController>
    {
        public MoveToMarketState(DeliveryController owner, StateMachine<DeliveryController> machine)
            : base(owner, machine) { }

        public override void Enter() { /* TODO */ }
        public override void Tick(float deltaTime) { /* TODO */ }
        public override void Exit() { /* TODO */ }
    }

    public class DeliverState : BaseState<DeliveryController>
    {
        public DeliverState(DeliveryController owner, StateMachine<DeliveryController> machine)
            : base(owner, machine) { }

        public override void Enter() { /* TODO */ }
        public override void Tick(float deltaTime) { /* TODO */ }
        public override void Exit() { /* TODO */ }
    }
}
