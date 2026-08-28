#region

using Template.Content.Scripts.Card.Blackboard;
using Template.Content.Scripts.Card.Fuzzy;

#endregion

namespace Template.Content.Scripts.Card.Fsm
{
    /// <summary>
    ///     Manages turns. States own the logic. Constructor Dependency Injection keeps them testable.
    /// </summary>
    internal sealed class FSM
    {
        // Private properties for this class
        private readonly GameBlackboard m_Blackboard;
        private readonly AIFuzzyBrain m_Fuzzy;
        private ICardRoundState m_Current;

        /// <summary>The state currently running, or null once the round has ended.</summary>
        public ICardRoundState Current => m_Current;

        // Properties filled when the object is constructed.
        public FSM(GameBlackboard blackboard, AIFuzzyBrain fuzzy)
        {
            m_Blackboard = blackboard;
            m_Fuzzy = fuzzy;
        }

        /// <summary>
        ///     Exits the current state and enters the next state.
        /// </summary>
        /// <param name="next">The state queued to be entered.</param>
        public void SetState(ICardRoundState next)
        {
            m_Current?.Exit();
            m_Current = next;
            m_Current?.Enter();
        }

        public void Tick() => m_Current?.Tick();
    }
}