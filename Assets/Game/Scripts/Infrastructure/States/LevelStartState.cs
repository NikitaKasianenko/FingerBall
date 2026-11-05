using System.Collections.Generic;
using Game.Infrastructure.StateMachine;
using Game.Level;
using Game.UI;

namespace Game.Infrastructure.States
{
    public class LevelStartState : IPayloadedState<int>
    {
        private readonly GameStateMachine _stateMachine;
        private readonly LevelSession _session;
        private readonly IReadOnlyList<IResettable> _resettables;
        private readonly GameHud _hud;

        public LevelStartState(GameStateMachine stateMachine, LevelSession session, List<IResettable> resettables,
            GameHud hud)
        {
            _stateMachine = stateMachine;
            _session = session;
            _resettables = resettables;
            _hud = hud;
        }

        public void Enter(int levelIndex)
        {
            _session.Load(levelIndex);
            foreach (IResettable resettable in _resettables)
                resettable.ResetState();

            _hud.ShowLevel(levelIndex + 1, _session.Mass.UsableFraction);
            _session.Start();
            _stateMachine.Enter<AdvanceState>();
        }

        public void Exit()
        {
        }
    }
}
