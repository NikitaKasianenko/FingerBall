using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Ball;
using Game.Infrastructure.StateMachine;
using Game.Input;
using Game.Level;
using Game.Shooting;
using Game.Simulation;
using Game.UI;
using UnityEngine;

namespace Game.Infrastructure.States
{
    public class AimState : FlowState, IState
    {
        private readonly GameStateMachine _stateMachine;
        private readonly LevelSession _session;
        private readonly ShotCharger _charger;
        private readonly IInputService _input;
        private readonly BallView _ball;
        private readonly ShotView _shot;
        private readonly GameHud _hud;

        public AimState(GameStateMachine stateMachine, LevelSession session, ShotCharger charger, IInputService input,
            BallView ball, ShotView shot, GameHud hud)
        {
            _stateMachine = stateMachine;
            _session = session;
            _charger = charger;
            _input = input;
            _ball = ball;
            _shot = shot;
            _hud = hud;
        }

        public void Enter()
        {
            _ball.SetIdle(true);
            _hud.SetHintVisible(_session.ShotsFired == 0);
            AimAsync(BeginFlow()).Forget();
        }

        public override void Exit()
        {
            base.Exit();
            _charger.Cancel();
        }

        private async UniTaskVoid AimAsync(CancellationToken cancellationToken)
        {
            await _input.WaitForPressAsync(cancellationToken);

            _ball.SetIdle(false);
            _hud.SetHintVisible(false);
            _charger.Begin();
            ShowCharge();

            while (true)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                if (!_input.IsPointerHeld)
                    break;

                _charger.Charge(Time.deltaTime);
                ShowCharge();
                if (_charger.IsDepleted)
                {
                    _stateMachine.Enter<DefeatState, LevelResult>(LevelResult.Overcharged);
                    return;
                }
            }

            ShotPlan plan = _charger.Fire(_ball.Distance);
            _stateMachine.Enter<ShotState, ShotPlan>(plan);
        }

        private void ShowCharge()
        {
            BallMass mass = _session.Mass;
            float ballRadius = mass.BallRadius;
            float shotRadius = mass.ShotRadius;

            _ball.SetRadius(ballRadius);
            _shot.ShowCharging(_session.Axis.ToWorld(_ball.Distance + ballRadius + shotRadius, shotRadius), shotRadius);
            _hud.SetMass(mass.UsableFraction);
        }
    }
}
