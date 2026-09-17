namespace StealthShooter.AI
{
    public interface IState
    {
        void Enter();
        void Execute();
        void Exit();
    }
}