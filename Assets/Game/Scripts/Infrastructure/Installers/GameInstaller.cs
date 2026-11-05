using Game.Audio;
using Game.Ball;
using Game.CameraLogic;
using Game.Infrastructure.StateMachine;
using Game.Infrastructure.States;
using Game.Level;
using Game.Obstacles;
using Game.Shooting;
using Game.UI;
using UnityEngine;
using Zenject;

namespace Game.Infrastructure.Installers
{
    public class GameInstaller : MonoInstaller
    {
        [Header("Scene")]
        [SerializeField] private BallView _ball;
        [SerializeField] private ShotView _shot;
        [SerializeField] private ObstacleField _obstacles;
        [SerializeField] private House _house;
        [SerializeField] private PathView _path;
        [SerializeField] private CameraDirector _cameraDirector;
        [SerializeField] private GameHud _hud;
        [SerializeField] private AudioPlayer _audioPlayer;

        public override void InstallBindings()
        {
            BindStateMachine();
            BindServices();
            BindSceneObjects();
        }

        private void BindStateMachine()
        {
            Container.BindInterfacesTo<GameBootstrapper>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameStateMachine>().AsSingle();
            Container.Bind<StatesFactory>().AsSingle();

            Container.Bind<BootstrapState>().AsSingle();
            Container.Bind<LevelStartState>().AsSingle();
            Container.Bind<AdvanceState>().AsSingle();
            Container.Bind<AimState>().AsSingle();
            Container.Bind<ShotState>().AsSingle();
            Container.Bind<FinishState>().AsSingle();
            Container.Bind<DefeatState>().AsSingle();
            Container.Bind<LevelEndState>().AsSingle();
        }

        private void BindServices()
        {
            Container.Bind<LevelSession>().AsSingle();
            Container.Bind<ShotCharger>().AsSingle();
            Container.BindInterfacesTo<AudioService>().AsSingle();
        }

        private void BindSceneObjects()
        {
            Container.BindInterfacesAndSelfTo<ObstacleField>().FromInstance(_obstacles);
            Container.BindInterfacesAndSelfTo<BallView>().FromInstance(_ball);
            Container.BindInterfacesAndSelfTo<ShotView>().FromInstance(_shot);
            Container.BindInterfacesAndSelfTo<House>().FromInstance(_house);
            Container.BindInterfacesAndSelfTo<PathView>().FromInstance(_path);
            Container.BindInterfacesAndSelfTo<CameraDirector>().FromInstance(_cameraDirector);
            Container.BindInstances(_hud, _audioPlayer);
        }
    }
}
