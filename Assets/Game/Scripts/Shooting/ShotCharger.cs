using System;
using Game.Level;
using Game.Simulation;

namespace Game.Shooting
{
    public class ShotCharger
    {
        private readonly LevelSession _session;
        private bool _isCharging;

        public event Action ChargeStarted;
        public event Action<float> ChargeChanged;
        public event Action ChargeEnded;
        public event Action Fired;

        public bool IsDepleted => _session.Mass.IsDepleted;

        public ShotCharger(LevelSession session) =>
            _session = session;

        public void Begin()
        {
            _session.Mass.BeginCharge();
            _isCharging = true;
            ChargeStarted?.Invoke();
            ChargeChanged?.Invoke(_session.Mass.ChargeFraction);
        }

        public void Charge(float deltaTime)
        {
            _session.Mass.Charge(deltaTime);
            ChargeChanged?.Invoke(_session.Mass.ChargeFraction);
        }

        public ShotPlan Fire(float ballDistance)
        {
            BallMass mass = _session.Mass;
            ShotPlan plan = _session.Course.PlanShot(ballDistance, mass.BallRadius, mass.ShotVolume);
            mass.Release();
            _session.RegisterShot();
            End();
            Fired?.Invoke();
            return plan;
        }

        public void Cancel() =>
            End();

        private void End()
        {
            if (!_isCharging)
                return;

            _isCharging = false;
            ChargeEnded?.Invoke();
        }
    }
}
