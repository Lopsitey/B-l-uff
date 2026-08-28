namespace Template.Content.Scripts.Card.Fsm
{
    internal interface ICardRoundState
    {
        void Enter();
        void Tick();
        void Exit();
    }
}
