namespace Template.Content.Scripts.Card.Fsm
{
    /// <summary>
    ///     Manages turns. States own the logic. Constructor Dependency Injection keeps them testable.
    /// </summary>
    internal sealed class FSM
    {
        public ICardRoundState CurrentState { get; private set; }

        /// <summary>
        ///     Exits the current state and enters the next state.
        /// </summary>
        /// <param name="next">The state queued to be entered.</param>
        public void SetState(ICardRoundState next)
        {
            CurrentState?.Exit();
            CurrentState = next;
            CurrentState?.Enter();
        }
    }
}