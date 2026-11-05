using System;
using Game.Ball;
using Game.Configs;
using Game.Level;
using Game.Obstacles;
using Game.Shooting;
using Game.UI;
using UnityEngine;
using Zenject;

namespace Game.Audio
{
    public class AudioService : IInitializable, IDisposable
    {
        private readonly AudioConfig _config;
        private readonly AudioPlayer _player;
        private readonly LevelSession _session;
        private readonly ShotCharger _charger;
        private readonly ObstacleField _obstacles;
        private readonly BallView _ball;
        private readonly House _house;
        private readonly GameHud _hud;

        private float _lastPopTime = float.MinValue;

        public AudioService(AudioConfig config, AudioPlayer player, LevelSession session, ShotCharger charger,
            ObstacleField obstacles, BallView ball, House house, GameHud hud)
        {
            _config = config;
            _player = player;
            _session = session;
            _charger = charger;
            _obstacles = obstacles;
            _ball = ball;
            _house = house;
            _hud = hud;
        }

        public void Initialize()
        {
            _session.Started += OnLevelStarted;
            _session.Ended += OnLevelEnded;
            _charger.ChargeStarted += OnChargeStarted;
            _charger.ChargeChanged += OnChargeChanged;
            _charger.ChargeEnded += OnChargeEnded;
            _charger.Fired += OnFired;
            _obstacles.Impacted += OnImpacted;
            _obstacles.ObstacleExploded += OnObstacleExploded;
            _ball.Landed += OnLanded;
            _ball.Deflating += OnDeflating;
            _ball.Popped += OnPopped;
            _house.DoorOpened += OnDoorOpened;
            _house.CelebrationStarted += OnCelebrationStarted;
            _hud.ButtonClicked += OnButtonClicked;
        }

        public void Dispose()
        {
            _session.Started -= OnLevelStarted;
            _session.Ended -= OnLevelEnded;
            _charger.ChargeStarted -= OnChargeStarted;
            _charger.ChargeChanged -= OnChargeChanged;
            _charger.ChargeEnded -= OnChargeEnded;
            _charger.Fired -= OnFired;
            _obstacles.Impacted -= OnImpacted;
            _obstacles.ObstacleExploded -= OnObstacleExploded;
            _ball.Landed -= OnLanded;
            _ball.Deflating -= OnDeflating;
            _ball.Popped -= OnPopped;
            _house.DoorOpened -= OnDoorOpened;
            _house.CelebrationStarted -= OnCelebrationStarted;
            _hud.ButtonClicked -= OnButtonClicked;
        }

        private void OnLevelStarted() =>
            _player.StopLoop();

        private void OnLevelEnded(LevelResult result)
        {
            _player.StopLoop();
            if (result != LevelResult.Win)
                _player.Play(_config.Lose);
        }

        private void OnChargeStarted() =>
            _player.StartLoop(_config.ChargeLoop, _config.ChargeVolume);

        private void OnChargeChanged(float charge) =>
            _player.SetLoopPitch(Mathf.Lerp(_config.ChargePitchMin, _config.ChargePitchMax, charge));

        private void OnChargeEnded() =>
            _player.StopLoop();

        private void OnFired() =>
            _player.Play(_config.Shoot);

        private void OnImpacted() =>
            _player.Play(_config.Impact);

        private void OnObstacleExploded()
        {
            if (Time.time - _lastPopTime < _config.MinPopInterval)
                return;

            _lastPopTime = Time.time;
            _player.Play(_config.ObstaclePop);
        }

        private void OnLanded() =>
            _player.Play(_config.Hop);

        private void OnDeflating() =>
            _player.Play(_config.Deflate);

        private void OnPopped() =>
            _player.Play(_config.BallPop);

        private void OnDoorOpened() =>
            _player.Play(_config.DoorOpen);

        private void OnCelebrationStarted() =>
            _player.Play(_config.Win);

        private void OnButtonClicked() =>
            _player.Play(_config.Click);
    }
}
