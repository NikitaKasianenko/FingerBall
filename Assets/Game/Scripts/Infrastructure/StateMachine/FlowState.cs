using System.Threading;

namespace Game.Infrastructure.StateMachine
{
    public abstract class FlowState : IExitableState
    {
        private CancellationTokenSource _cancellation;

        public virtual void Exit()
        {
            _cancellation?.Cancel();
            _cancellation?.Dispose();
            _cancellation = null;
        }

        protected CancellationToken BeginFlow()
        {
            _cancellation = new CancellationTokenSource();
            return _cancellation.Token;
        }
    }
}
