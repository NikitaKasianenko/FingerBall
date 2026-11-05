using Zenject;

namespace Game.Infrastructure.Installers
{
    public class BootInstaller : MonoInstaller
    {
        public override void InstallBindings() =>
            Container.BindInterfacesTo<BootEntryPoint>().AsSingle();
    }
}
