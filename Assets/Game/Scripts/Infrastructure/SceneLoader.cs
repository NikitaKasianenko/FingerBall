using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Game.Infrastructure
{
    public class SceneLoader
    {
        public UniTask LoadAsync(string sceneName, CancellationToken cancellationToken = default) =>
            SceneManager.LoadSceneAsync(sceneName).ToUniTask(cancellationToken: cancellationToken);
    }
}
