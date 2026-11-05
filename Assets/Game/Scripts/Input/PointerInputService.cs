using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;

namespace Game.Input
{
    public class PointerInputService : IInputService
    {
        public bool IsPointerHeld =>
            Pointer.current != null && Pointer.current.press.isPressed;

        public UniTask WaitForPressAsync(CancellationToken cancellationToken) =>
            UniTask.WaitUntil(IsPressedThisFrame, cancellationToken: cancellationToken);

        private static bool IsPressedThisFrame() =>
            Pointer.current != null && Pointer.current.press.wasPressedThisFrame;
    }
}
