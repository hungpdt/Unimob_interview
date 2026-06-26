namespace Farm
{
    public abstract class BaseState<TOwner>
    {
        protected readonly TOwner _owner;
        protected readonly StateMachine<TOwner> _machine;

        protected BaseState(TOwner owner, StateMachine<TOwner> machine)
        {
            _owner = owner;
            _machine = machine;
        }

        public abstract void Enter();
        public abstract void Tick(float deltaTime);
        public abstract void Exit();

        // Optional hook for animation events (e.g. harvest/receive complete).
        public virtual void OnAnimationEvent(string id) { }
    }

    public class StateMachine<TOwner>
    {
        public BaseState<TOwner> Current { get; private set; }

        // TODO: Current?.Exit(); Current = next; next.Enter();
        public void ChangeState(BaseState<TOwner> next)
        {
            // TODO: implement
        }

        public void Tick(float deltaTime)
        {
            Current?.Tick(deltaTime);
        }
    }
}
