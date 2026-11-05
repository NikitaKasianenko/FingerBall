using Game.Ball;
using Game.Configs;
using Game.Level;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

namespace Game.CameraLogic
{
    public class CameraDirector : MonoBehaviour, IResettable
    {
        private const int ActivePriority = 10;
        private const int InactivePriority = 0;

        [SerializeField] private CinemachineCamera _gameplayCamera;
        [SerializeField] private Transform _focus;

        private CameraConfig _config;
        private LevelSession _session;
        private BallView _ball;
        private House _house;
        private float _anchorX;
        private float _minZ;
        private float _maxZ;

        [Inject]
        public void Construct(CameraConfig config, LevelSession session, BallView ball, House house)
        {
            _config = config;
            _session = session;
            _ball = ball;
            _house = house;
        }

        public void ResetState()
        {
            Vector2 start = _session.Axis.Start;
            Vector2 end = _session.Axis.End;
            _anchorX = (start.x + end.x) * 0.5f;
            _minZ = Mathf.Min(start.y, end.y);
            _maxZ = Mathf.Max(_minZ, Mathf.Max(start.y, end.y) - _config.StopBeforeEnd);

            _focus.position = new Vector3(_anchorX, 0f, _minZ);
            _gameplayCamera.PreviousStateIsValid = false;
            ShowGameplay();
        }

        public void ShowGameplay() =>
            Activate(_gameplayCamera);

        public void ShowFinish() =>
            Activate(_house.FinishCamera);

        private void LateUpdate() =>
            _focus.position = new Vector3(_anchorX, 0f, Mathf.Clamp(_ball.transform.position.z, _minZ, _maxZ));

        private void Activate(CinemachineCamera activeCamera)
        {
            _gameplayCamera.Priority = activeCamera == _gameplayCamera ? ActivePriority : InactivePriority;
            _house.FinishCamera.Priority = activeCamera == _house.FinishCamera ? ActivePriority : InactivePriority;
        }
    }
}
