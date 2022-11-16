namespace GameCore {
    public class StateMachine<T> {
        public State<T> CurrentState { get; private set; }
        public readonly T Ctx;

        public StateMachine(T ctx) {
            Ctx = ctx;
            CurrentState = null;
        }

        public void ChangeState(State<T> newState) {
            CurrentState?.ExitState(Ctx);
            CurrentState = newState;
            newState.EnterState(Ctx);
        }

        public void Update() {
            CurrentState?.UpdateState(Ctx);
        }
    }

    public abstract class State<T> {
        public abstract void EnterState(T ctx);
        public abstract void ExitState(T ctx);
        public abstract void UpdateState(T ctx);
    }
}