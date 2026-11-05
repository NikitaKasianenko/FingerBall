using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Input
{
    public interface IInputService
    {
        bool IsPointerHeld { get; }

        UniTask WaitForPressAsync(CancellationToken cancellationToken);
    }
}
