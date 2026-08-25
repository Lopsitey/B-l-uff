using Bluff.Card.Blackboard;
using Bluff.Card.Fuzzy;

namespace Bluff.Card.Fsm
{
    /// <summary>
    /// Thin turn driver. States own the logic. Constructor DI keeps them testable.
    /// </summary>
    public sealed class CardRoundFsm
    {
        private readonly CardRoundBlackboard m_Blackboard;
        private readonly BluffFuzzyEvaluator m_Fuzzy;
        private ICardRoundState m_Current;

        public CardRoundFsm(CardRoundBlackboard blackboard, BluffFuzzyEvaluator fuzzy)
        {
            m_Blackboard = blackboard;
            m_Fuzzy = fuzzy;
        }

        public CardRoundBlackboard Blackboard => m_Blackboard;
        public BluffFuzzyEvaluator Fuzzy => m_Fuzzy;
        public ICardRoundState Current => m_Current;

        public void SetState(ICardRoundState next)
        {
            if (m_Current != null)
            {
                m_Current.Exit();
            }

            m_Current = next;

            if (m_Current != null)
            {
                m_Current.Enter();
            }
        }

        public void Tick()
        {
            if (m_Current != null)
            {
                m_Current.Tick();
            }
        }
    }
}
