using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Infrastructure.StateMachine;
using Game.Level;
using Game.Obstacles;
using Game.Shooting;
using Game.Simulation;
using UnityEngine;

namespace Game.Infrastructure.States
{
    public class ShotState : FlowState, IPayloadedState<ShotPlan>
    {
        private readonly GameStateMachine _stateMachine;
        private readonly LevelSession _session;
        private readonly ShotView _shot;
        private readonly ObstacleField _obstacles;
        private readonly List<InfectedObstacle> _infected = new();

        public ShotState(GameStateMachine stateMachine, LevelSession session, ShotView shot, ObstacleField obstacles)
        {
            _stateMachine = stateMachine;
            _session = session;
            _shot = shot;
            _obstacles = obstacles;
        }

        public void Enter(ShotPlan plan) =>
            ShootAsync(plan, BeginFlow()).Forget();

        private async UniTaskVoid ShootAsync(ShotPlan plan, CancellationToken cancellationToken)
        {
            Vector3 impactPoint = _session.Axis.ToWorld(plan.ImpactDistance, plan.ShotRadius);
            await _shot.FlyAsync(impactPoint, cancellationToken);
            _shot.Hide();

            if (plan.HasTarget)
            {
                _session.Course.Infect(plan, _infected);
                await _obstacles.ExplodeAsync(_infected, impactPoint, cancellationToken);
            }

            _stateMachine.Enter<AdvanceState>();
        }
    }
}
