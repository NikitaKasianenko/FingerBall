using UnityEngine;
using Zenject;

namespace Game.Configs
{
    [CreateAssetMenu(fileName = "GameConfigInstaller", menuName = "Game/Game Config Installer")]
    public class GameConfigInstaller : ScriptableObjectInstaller<GameConfigInstaller>
    {
        [SerializeField] private BallConfig _ball;
        [SerializeField] private ShotConfig _shot;
        [SerializeField] private InfectionConfig _infection;
        [SerializeField] private LevelConfig _level;
        [SerializeField] private BalanceConfig _balance;
        [SerializeField] private MotionConfig _motion;
        [SerializeField] private ExplosionConfig _explosion;
        [SerializeField] private HouseConfig _house;
        [SerializeField] private CameraConfig _camera;
        [SerializeField] private AudioConfig _audio;

        public override void InstallBindings() =>
            Container.BindInstances(_ball, _shot, _infection, _level, _balance, _motion, _explosion, _house, _camera,
                _audio);
    }
}
