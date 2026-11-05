using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Ball;
using Game.Infrastructure.StateMachine;
using Game.Level;

namespace Game.Infrastructure.States
{
    public class AdvanceState : FlowState, IState
    {
        private readonly GameStateMachine _stateMachine;
        private readonly LevelSession _session;
        private readonly BallView _ball;

        public AdvanceState(GameStateMachine stateMachine, LevelSession session, BallView ball)
        {
            _stateMachine = stateMachine;
            _session = session;
            _ball = ball;
        }

        public void Enter() =>
            AdvanceAsync(BeginFlow()).Forget();

        private async UniTaskVoid AdvanceAsync(CancellationToken cancellationToken)
        {
            _ball.SetIdle(false);
            float stop = _session.Course.NextStop(_ball.Distance, _ball.Radius, out bool reachesDoor);
            await _ball.HopToAsync(stop, cancellationToken);

            if (reachesDoor)
                _stateMachine.Enter<FinishState>();
            else if (!_session.Mass.CanShoot)
                _stateMachine.Enter<DefeatState, LevelResult>(LevelResult.OutOfMass);
            else
                _stateMachine.Enter<AimState>();
        }
    }
}
