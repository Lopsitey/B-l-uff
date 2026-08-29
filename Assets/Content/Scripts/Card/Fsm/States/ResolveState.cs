#region

using Template.Content.Scripts.Card.Blackboard;
using Template.Content.Scripts.Card.Data;
using UnityEngine;

#endregion

namespace Template.Content.Scripts.Card.Fsm.States
{
    /// <summary>
    ///     Truth check, pile award/take, trust hit, win condition check, and turn swapping.
    ///     Central authority for all turn outcomes.
    /// </summary>
    internal sealed class ResolveState : ICardRoundState
    {
        private readonly FSM m_Fsm;
        private readonly GameBlackboard m_Board;

        public ResolveState(FSM fsm, GameBlackboard board)
        {
            m_Fsm = fsm;
            m_Board = board;
        }

        public void Enter()
        {
            Debug.Log(
                $"[CardRound] Resolve.Enter: pile = {m_Board.PileSize}, challenged = {m_Board.LastPlayWasChallenged}, trust = {m_Board.TrustTowardPlayer:0.00}");

            if (m_Board.LastPlayWasChallenged)
            {
                ResolveChallenge();
            }
        }

        public void Tick()
        {
            // 1. Check Win / Loss condition
            if (m_Board.PlayerHand.Count == 0)
            {
                Debug.Log("[CardRound] *** PLAYER WINS THE ROUND! (Hand is empty) ***");
                return;
            }

            if (m_Board.OpponentHand.Count == 0)
            {
                Debug.Log($"[CardRound] *** {m_Board.GetOpponentLabel()} WINS THE ROUND! (Hand is empty) ***");
                return;
            }

            // 2. If round continues, swap active seat and return to DecideState
            m_Board.SwapActiveTurn();
            m_Fsm.SetState(new DecideState(m_Fsm, m_Board));
        }

        public void Exit()
        {
        }

        private void ResolveChallenge()
        {
            var activeSeat = m_Board.ActiveTurn;
            var defendingSeat = activeSeat == TurnUser.Player ? TurnUser.Opponent : TurnUser.Player;

            // Determine if the active seat was telling the truth
            var wasBluff = false;
            for (var i = 0; i < m_Board.LastPlayedCards.Count; i++)
            {
                if (m_Board.LastPlayedCards[i].Colour != m_Board.TargetColour)
                {
                    wasBluff = true;
                    break;
                }
            }

            if (wasBluff)
            {
                Debug.Log($"[CardRound] Challenge SUCCEEDED! {activeSeat} was caught bluffing!");

                if (activeSeat == TurnUser.Player)
                {
                    m_Board.DecrementTrust();
                }

                // Apply debuff of the revealed fake card
                if (m_Board.LastPlayedCards.Count > 0)
                {
                    Debuffs.ApplyCardDebuff(m_Board.LastPlayedCards[0]);
                }

                // Penalty: Liar draws 3 penalty cards from the draw stack
                m_Board.DrawExtraCards(activeSeat, 3);
            }
            else
            {
                Debug.Log($"[CardRound] Challenge FAILED! {activeSeat} was telling the truth!");

                // Penalty: Wrongful challenger draws 3 penalty cards from the draw stack
                m_Board.DrawExtraCards(defendingSeat, 3);
            }

            // Clear the pot on challenge resolution
            m_Board.PileSize = 0;
            m_Board.LastPlayedCards.Clear();
        }
    }
}
