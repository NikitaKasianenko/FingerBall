using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Ball;
using Game.Configs;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

namespace Game.Level
{
    public class House : MonoBehaviour, IResettable
    {
        private const int SquashVibrato = 4;
        private const float SquashElasticity = 0.6f;

        [SerializeField] private Transform _body;
        [SerializeField] private Transform _doorPivot;
        [SerializeField] private ParticleSystem _confetti;
        [SerializeField] private CinemachineCamera _finishCamera;

        private HouseConfig _config;
        private LevelSession _session;
        private BallView _ball;
        private bool _isOpen;

        public event Action DoorOpened;
        public event Action CelebrationStarted;

        public CinemachineCamera FinishCamera => _finishCamera;

        [Inject]
        public void Construct(HouseConfig config, LevelSession session, BallView ball)
        {
            _config = config;
            _session = session;
            _ball = ball;
        }

        public void ResetState()
        {
            transform.SetPositionAndRotation(_session.Axis.ToWorld(_session.Axis.Length, 0f),
                Quaternion.LookRotation(_session.Axis.WorldDirection, Vector3.up));

            _body.DOKill();
            _doorPivot.DOKill();
            _body.localScale = Vector3.one;
            _doorPivot.localRotation = Quaternion.identity;
            _confetti.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _isOpen = false;
        }

        public UniTask CelebrateAsync(CancellationToken cancellationToken)
        {
            PlayCelebration();
            CelebrationStarted?.Invoke();
            return UniTask.Delay(TimeSpan.FromSeconds(_config.CelebrationDuration), cancellationToken: cancellationToken);
        }

        private void PlayCelebration()
        {
            _doorPivot.DOKill();
            _doorPivot.DOLocalRotate(Vector3.zero, _config.CloseDuration).SetEase(Ease.InQuad);

            var squash = new Vector3(-_config.Squash * 0.5f, _config.Squash, -_config.Squash * 0.5f);
            _body.DOKill(complete: true);
            _body.DOPunchScale(squash, _config.SquashDuration, SquashVibrato, SquashElasticity);
            _confetti.Play();
        }

        private void Update()
        {
            if (_isOpen || !IsBallClose())
                return;

            _isOpen = true;
            _doorPivot.DOLocalRotate(new Vector3(0f, -_config.OpenAngle, 0f), _config.OpenDuration).SetEase(Ease.OutQuad);
            DoorOpened?.Invoke();
        }

        private bool IsBallClose()
        {
            Vector3 delta = _ball.transform.position - transform.position;
            delta.y = 0f;
            return delta.sqrMagnitude <= _config.OpenDistance * _config.OpenDistance;
        }
    }
}
