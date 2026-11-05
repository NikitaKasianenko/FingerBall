using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Ball;
using Game.CameraLogic;
using Game.Configs;
using Game.Infrastructure.StateMachine;
using Game.Level;

namespace Game.Infrastructure.States
{
    public class FinishState : FlowState, IState
    {
        private readonly GameStateMachine _stateMachine;
        private readonly LevelSession _session;
        private readonly BallView _ball;
        private readonly House _house;
        private readonly CameraDirector _camera;
        private readonly MotionConfig _motion;
        private readonly CameraConfig _cameraConfig;

        public FinishState(GameStateMachine stateMachine, LevelSession session, BallView ball, House house,
            CameraDirector camera, MotionConfig motion, CameraConfig cameraConfig)
        {
            _stateMachine = stateMachine;
            _session = session;
            _ball = ball;
            _house = house;
            _camera = camera;
            _motion = motion;
            _cameraConfig = cameraConfig;
        }

        public void Enter() =>
            FinishAsync(BeginFlow()).Forget();

        private async UniTaskVoid FinishAsync(CancellationToken cancellationToken)
        {
            _camera.ShowFinish();
            await UniTask.Delay(TimeSpan.FromSeconds(_cameraConfig.FinishBlendTime), cancellationToken: cancellationToken);

            await _ball.RollIntoAsync(_session.Axis.Length + _motion.EnterDepth, cancellationToken);
            await _house.CelebrateAsync(cancellationToken);

            _camera.ShowGameplay();
            await UniTask.Delay(TimeSpan.FromSeconds(_cameraConfig.ReturnBlendTime), cancellationToken: cancellationToken);

            _stateMachine.Enter<LevelEndState, LevelResult>(LevelResult.Win);
        }
    }
}
