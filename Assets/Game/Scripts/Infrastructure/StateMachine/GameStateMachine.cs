using System;

namespace Game.Infrastructure.StateMachine
{
    public class GameStateMachine : IDisposable
    {
        private readonly StatesFactory _statesFactory;
        private IExitableState _activeState;

        public GameStateMachine(StatesFactory statesFactory) =>
            _statesFactory = statesFactory;

        public void Enter<TState>() where TState : class, IState =>
            ChangeState<TState>().Enter();

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload> =>
            ChangeState<TState>().Enter(payload);

        public void Dispose()
        {
            _activeState?.Exit();
            _activeState = null;
        }

        private TState ChangeState<TState>() where TState : class, IExitableState
        {
            _activeState?.Exit();

            TState state = _statesFactory.GetState<TState>();
            _activeState = state;
            return state;
        }
    }
}
