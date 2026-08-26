using Content.Scripts.Card.Blackboard;
using UnityEngine;

namespace Bluff.Card.Fsm.States
{
    /// <summary>
    /// Truth check, pile take, trust hit, win/lose scratch values for combat later.
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
            Debug.Log($"[CardRound] Resolve.Enter buffScratch={m_Board.PendingCombatBuffStrength}");
            // On caught lie: m_Board.ApplyCaughtLieTrustHit(enemyIndex);
            // Optional UI: "-1 trust" / "X will remember that" (cosmetic only).
            // Win path: bump PendingCombatBuffStrength (bigger for multi-card risky lies).
            // Lose path: bump PendingCombatDebuffStrength.
            // Do not start combat from here.
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
