using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Infrastructure.StateMachine;
using Game.Level;
using Game.UI;

namespace Game.Infrastructure.States
{
    public class LevelEndState : FlowState, IPayloadedState<LevelResult>
    {
        private readonly GameStateMachine _stateMachine;
        private readonly LevelSession _session;
        private readonly GameHud _hud;

        public LevelEndState(GameStateMachine stateMachine, LevelSession session, GameHud hud)
        {
            _stateMachine = stateMachine;
            _session = session;
            _hud = hud;
        }

        public void Enter(LevelResult result)
        {
            _session.End(result);
            _hud.ShowResult(result);
            WaitForChoiceAsync(BeginFlow()).Forget();
        }

        public override void Exit()
        {
            base.Exit();
            _hud.HideResult();
        }

        private async UniTaskVoid WaitForChoiceAsync(CancellationToken cancellationToken)
        {
            LevelChoice choice = await _hud.WaitForChoiceAsync(cancellationToken);
            int nextLevel = choice == LevelChoice.Next ? _session.Index + 1 : _session.Index;
            _stateMachine.Enter<LevelStartState, int>(nextLevel);
        }
    }
}
