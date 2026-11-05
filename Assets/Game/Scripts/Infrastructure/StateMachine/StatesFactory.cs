using Zenject;

namespace Game.Infrastructure.StateMachine
{
    public class StatesFactory
    {
        private readonly DiContainer _container;

        public StatesFactory(DiContainer container) =>
            _container = container;

        public TState GetState<TState>() where TState : class, IExitableState =>
            _container.Resolve<TState>();
    }
}
