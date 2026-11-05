using Game.Infrastructure.StateMachine;
using Game.Infrastructure.States;
using Zenject;

namespace Game.Infrastructure
{
    public class GameBootstrapper : IInitializable
    {
        private readonly GameStateMachine _stateMachine;

        public GameBootstrapper(GameStateMachine stateMachine) =>
            _stateMachine = stateMachine;

        public void Initialize() =>
            _stateMachine.Enter<BootstrapState>();
    }
}
