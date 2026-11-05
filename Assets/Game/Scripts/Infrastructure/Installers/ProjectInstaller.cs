using Game.Input;
using Zenject;

namespace Game.Infrastructure.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<SceneLoader>().AsSingle();
            Container.Bind<IInputService>().To<PointerInputService>().AsSingle();
        }
    }
}
