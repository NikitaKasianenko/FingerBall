using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Game.Infrastructure
{
    public class BootEntryPoint : IInitializable
    {
        private const int TargetFrameRate = 60;
        private const int TweenersCapacity = 800;
        private const int SequencesCapacity = 20;

        private readonly SceneLoader _sceneLoader;

        public BootEntryPoint(SceneLoader sceneLoader) =>
            _sceneLoader = sceneLoader;

        public void Initialize()
        {
            Application.targetFrameRate = TargetFrameRate;
            DOTween.Init().SetCapacity(TweenersCapacity, SequencesCapacity);

            _sceneLoader.LoadAsync(SceneNames.Game).Forget();
        }
    }
}
