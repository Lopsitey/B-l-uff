using Bluff.Card.Blackboard;
using Bluff.Card.Fuzzy;
using UnityEngine;

namespace Bluff.Card.Fsm.States
{
    /// <summary>
    /// Opponents decide whether to call. Fuzzy suspicion vs hidden trust.
    /// </summary>
    public sealed class AwaitChallengeState : ICardRoundState
    {
        private readonly CardRoundFsm m_Fsm;
        private readonly CardRoundBlackboard m_Board;
        private readonly BluffFuzzyEvaluator m_Fuzzy;

        public AwaitChallengeState(CardRoundFsm fsm, CardRoundBlackboard board, BluffFuzzyEvaluator fuzzy)
        {
            m_Fsm = fsm;
            m_Board = board;
            m_Fuzzy = fuzzy;
        }

        public void Enter()
        {
            Debug.Log("[CardRound] AwaitChallenge.Enter");
        }

        public void Tick()
        {
            // TODO: for each eligible enemy, EvaluateChallengeChance; if call, Resolve
        }

        public void Exit()
        {
        }
    }
}
