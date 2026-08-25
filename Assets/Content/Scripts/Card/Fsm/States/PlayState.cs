using Bluff.Card.Blackboard;
using UnityEngine;

namespace Bluff.Card.Fsm.States
{
    /// <summary>
    /// Commit cards to the pile and announce the claim. Multi-card lies go here later.
    /// </summary>
    public sealed class PlayState : ICardRoundState
    {
        private readonly CardRoundFsm m_Fsm;
        private readonly CardRoundBlackboard m_Board;

        public PlayState(CardRoundFsm fsm, CardRoundBlackboard board)
        {
            m_Fsm = fsm;
            m_Board = board;
        }

        public void Enter()
        {
            Debug.Log($"[CardRound] Play.Enter pile={m_Board.PileSize}");
        }

        public void Tick()
        {
            // TODO: animate play, then AwaitChallengeState
        }

        public void Exit()
        {
        }
    }
}
