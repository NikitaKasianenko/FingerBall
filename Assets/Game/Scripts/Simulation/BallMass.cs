using Game.Configs;
using UnityEngine;

namespace Game.Simulation
{
    public class BallMass
    {
        private readonly BallConfig _ball;
        private readonly ShotConfig _shot;
        private float _holdTime;

        public BallMass(BallConfig ball, ShotConfig shot)
        {
            _ball = ball;
            _shot = shot;
            Reset();
        }

        public float BallVolume { get; private set; }
        public float ShotVolume { get; private set; }
        public float BallRadius => SizeModel.RadiusFromVolume(BallVolume);
        public float ShotRadius => SizeModel.RadiusFromVolume(ShotVolume);
        public float UsableFraction => Mathf.Clamp01((BallVolume - _ball.CriticalVolume) / _ball.UsableVolume);
        public float ChargeFraction => Mathf.Clamp01(ShotVolume / _ball.UsableVolume);
        public bool IsDepleted => BallVolume <= _ball.CriticalVolume;
        public bool CanShoot => BallVolume - _shot.MinVolume > _ball.CriticalVolume;

        public void Reset()
        {
            BallVolume = _ball.StartVolume;
            ShotVolume = 0f;
            _holdTime = 0f;
        }

        public void BeginCharge()
        {
            _holdTime = 0f;
            Transfer(_shot.MinVolume);
        }

        public void Charge(float deltaTime)
        {
            _holdTime += deltaTime;
            Transfer(_shot.ChargeRateAt(_holdTime) * deltaTime);
        }

        public void Release() =>
            ShotVolume = 0f;

        private void Transfer(float volume)
        {
            float transferred = Mathf.Min(volume, BallVolume - _ball.CriticalVolume);
            BallVolume -= transferred;
            ShotVolume += transferred;
        }
    }
}
