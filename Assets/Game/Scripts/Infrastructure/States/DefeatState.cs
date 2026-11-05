using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Ball;
using Game.Configs;
using Game.Infrastructure.StateMachine;
using Game.Level;
using Game.Shooting;

namespace Game.Infrastructure.States
{
    public class DefeatState : FlowState, IPayloadedState<LevelResult>
    {
        private readonly GameStateMachine _stateMachine;
        private readonly BallView _ball;
        private readonly ShotView _shot;
        private readonly MotionConfig _motion;

        public DefeatState(GameStateMachine stateMachine, BallView ball, ShotView shot, MotionConfig motion)
        {
            _stateMachine = stateMachine;
            _ball = ball;
            _shot = shot;
            _motion = motion;
        }

        public void Enter(LevelResult result) =>
            DefeatAsync(result, BeginFlow()).Forget();

        private async UniTaskVoid DefeatAsync(LevelResult result, CancellationToken cancellationToken)
        {
            _shot.Hide();
            await _ball.DeflateAsync(cancellationToken);
            await UniTask.Delay(TimeSpan.FromSeconds(_motion.DefeatPause), cancellationToken: cancellationToken);
            _stateMachine.Enter<LevelEndState, LevelResult>(result);
        }
    }
}
