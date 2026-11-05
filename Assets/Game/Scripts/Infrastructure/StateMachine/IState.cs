namespace Game.Infrastructure.StateMachine
{
    public interface IState : IExitableState
    {
        void Enter();
    }
}
