using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Configs;
using Game.Effects;
using Game.Level;
using Game.Simulation;
using UnityEngine;
using Zenject;

namespace Game.Ball
{
    public class BallView : MonoBehaviour, IResettable
    {
        private const int DeflateVibrato = 5;

        [SerializeField] private Transform _body;
        [SerializeField] private ParticleBurst _landingDust;
        [SerializeField] private ParticleBurst _popBurst;

        private MotionConfig _motion;
        private ExplosionConfig _explosion;
        private LevelSession _session;
        private PathAxis _axis;
        private float _bounceTime;
        private bool _isIdle;
        private bool _isMoving;

        public event Action Landed;
        public event Action Deflating;
        public event Action Popped;

        public float Distance { get; private set; }
        public float Radius { get; private set; }

        [Inject]
        public void Construct(MotionConfig motion, ExplosionConfig explosion, LevelSession session)
        {
            _motion = motion;
            _explosion = explosion;
            _session = session;
        }

        public void ResetState()
        {
            _axis = _session.Axis;
            Distance = 0f;
            Radius = _session.Mass.BallRadius;
            _isIdle = false;
            _isMoving = false;
            _body.DOKill();
            gameObject.SetActive(true);
            Apply(0f, 0f, 1f);
        }

        public void SetRadius(float radius) =>
            Radius = radius;

        public void SetIdle(bool isIdle)
        {
            _isIdle = isIdle;
            _bounceTime = 0f;
        }

        public async UniTask HopToAsync(float targetDistance, CancellationToken cancellationToken)
        {
            float start = Distance;
            float length = targetDistance - start;
            if (length <= Mathf.Epsilon)
                return;

            int hopCount = Mathf.Max(1, Mathf.CeilToInt(length / _motion.HopLength));
            float segment = length / hopCount;
            _isMoving = true;
            for (int i = 0; i < hopCount; i++)
            {
                await HopAsync(start + segment * i, segment, cancellationToken);
                _landingDust.Burst(_axis.ToWorld(Distance, 0f), _explosion.ParticlesPerLanding);
                Landed?.Invoke();
            }

            _isMoving = false;
        }

        public async UniTask RollIntoAsync(float targetDistance, CancellationToken cancellationToken)
        {
            float start = Distance;
            float time = 0f;
            _isMoving = true;
            while (time < _motion.EnterDuration)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                time += Time.deltaTime;
                float progress = Mathf.Clamp01(time / _motion.EnterDuration);
                Distance = Mathf.Lerp(start, targetDistance, progress * progress);
                Apply(0f, 0f, Mathf.Lerp(1f, _motion.EnterEndScale, progress));
            }

            gameObject.SetActive(false);
        }

        public async UniTask DeflateAsync(CancellationToken cancellationToken)
        {
            _isMoving = true;
            Deflating?.Invoke();

            float wobble = Radius * 2f * _motion.DefeatWobble;
            _body.DOKill(complete: true);
            await _body.DOPunchScale(new Vector3(wobble, -wobble, wobble), _motion.DefeatWobbleDuration, DeflateVibrato)
                .ToUniTask(TweenCancelBehaviour.Kill, cancellationToken);
            await _body.DOScale(0f, _motion.DefeatShrinkDuration)
                .SetEase(Ease.InBack)
                .ToUniTask(TweenCancelBehaviour.Kill, cancellationToken);

            _popBurst.Burst(transform.position, _explosion.ParticlesPerBallPop);
            Popped?.Invoke();
            gameObject.SetActive(false);
        }

        private async UniTask HopAsync(float from, float segment, CancellationToken cancellationToken)
        {
            float heightScale = Mathf.Clamp01(segment / _motion.HopLength);
            float time = 0f;
            while (time < _motion.HopDuration)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                time += Time.deltaTime;
                float progress = Mathf.Clamp01(time / _motion.HopDuration);
                Distance = from + segment * progress;

                float height = _motion.HopHeight * heightScale * 4f * progress * (1f - progress);
                float stretch = _motion.HopStretch * heightScale * Mathf.Sin(progress * Mathf.PI);
                Apply(height, stretch, 1f);
            }
        }

        private void Update()
        {
            if (_isMoving || _motion == null)
                return;

            float height = 0f;
            if (_isIdle)
            {
                _bounceTime += Time.deltaTime;
                height = Mathf.Abs(Mathf.Sin(_bounceTime * _motion.IdleBounceFrequency * Mathf.PI)) * _motion.IdleBounceHeight;
            }

            Apply(height, 0f, 1f);
        }

        private void Apply(float height, float stretch, float scale)
        {
            float diameter = Radius * 2f * scale;
            float squeeze = 1f - stretch * 0.5f;
            transform.position = _axis.ToWorld(Distance, Radius * scale + height);
            _body.localScale = new Vector3(diameter * squeeze, diameter * (1f + stretch), diameter * squeeze);
        }
    }
}
