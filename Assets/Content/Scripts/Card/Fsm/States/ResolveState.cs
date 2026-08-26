using Content.Scripts.Card.Blackboard;
using UnityEngine;

namespace Bluff.Card.Fsm.States
{
    /// <summary>
    /// Truth check, pile take, trust hit. Round stays on the card table.
    /// </summary>
    public sealed class ResolveState : ICardRoundState
    {
        private readonly CardRoundFsm m_Fsm;
        private readonly CardRoundBlackboard m_Board;

        public ResolveState(CardRoundFsm fsm, CardRoundBlackboard board)
        {
            m_Fsm = fsm;
            m_Board = board;
        }

        public void Enter()
        {
            Debug.Log($"[CardRound] Resolve.Enter pile={m_Board.PileSize}");
            // On caught lie: m_Board.ApplyCaughtLieTrustHit(enemyIndex);
            // Optional UI: "-1 trust" / "X will remember that" (cosmetic only).
            // Apply in-round curse / debuff effects here when potion rules land.
        }

        public void Tick()
        {
            // TODO: next turn DrawState, or end round
        }

        public void Exit()
        {
        }
    }
}
