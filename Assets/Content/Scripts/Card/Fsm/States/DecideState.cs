using Bluff.Card.Blackboard;
using Bluff.Card.Fuzzy;
using UnityEngine;

namespace Bluff.Card.Fsm.States
{
    /// <summary>
    /// AI or player chooses honest play vs bluff. Fuzzy feeds risk, not a bool.
    /// </summary>
    public sealed class DecideState : ICardRoundState
    {
        private readonly CardRoundFsm m_Fsm;
        private readonly CardRoundBlackboard m_Board;
        private readonly BluffFuzzyEvaluator m_Fuzzy;

        public DecideState(CardRoundFsm fsm, CardRoundBlackboard board, BluffFuzzyEvaluator fuzzy)
        {
            m_Fsm = fsm;
            m_Board = board;
            m_Fuzzy = fuzzy;
        }

        public void Enter()
        {
            Debug.Log("[CardRound] Decide.Enter");
            // Example read only. Real choice comes later.
            var risk = m_Fuzzy.EvaluateBluffRisk(m_Board, m_Board.CurrentPlayerIndex);
            Debug.Log($"[CardRound] sample bluff risk = {risk:0.00}");
        }

        public void Tick()
        {
            // TODO: pick cards + claim, then PlayState
        }

        public void Exit()
        {
        }
    }
}
