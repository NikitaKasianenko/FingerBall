using Game.Infrastructure.StateMachine;

namespace Game.Infrastructure.States
{
    public class BootstrapState : IState
    {
        private const int FirstLevel = 0;

        private readonly GameStateMachine _stateMachine;

        public BootstrapState(GameStateMachine stateMachine) =>
            _stateMachine = stateMachine;

        public void Enter() =>
            _stateMachine.Enter<LevelStartState, int>(FirstLevel);

        public void Exit()
        {
        }
    }
}
