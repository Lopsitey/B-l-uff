namespace Bluff.Card.Fsm
{
    public interface ICardRoundState
    {
        void Enter();
        void Tick();
        void Exit();
    }
}
