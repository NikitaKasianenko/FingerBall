using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Configs;
using Game.Level;
using UnityEngine;
using Zenject;

namespace Game.Shooting
{
    public class ShotView : MonoBehaviour, IResettable
    {
        [SerializeField] private Transform _body;

        private ShotConfig _config;

        [Inject]
        public void Construct(ShotConfig config) =>
            _config = config;

        public void ResetState() =>
            Hide();

        public void ShowCharging(Vector3 center, float radius)
        {
            transform.DOKill();
            gameObject.SetActive(true);
            transform.position = center;
            _body.localScale = Vector3.one * (radius * 2f);
        }

        public UniTask FlyAsync(Vector3 target, CancellationToken cancellationToken)
        {
            float duration = Vector3.Distance(transform.position, target) / _config.FlightSpeed;
            return transform.DOMove(target, duration)
                .SetEase(Ease.Linear)
                .ToUniTask(TweenCancelBehaviour.Kill, cancellationToken);
        }

        public void Hide()
        {
            transform.DOKill();
            gameObject.SetActive(false);
        }
    }
}
