#region

using Template.Content.Scripts.Card.Blackboard;
using Template.Content.Scripts.Card.Data;
using Template.Content.Scripts.Managers;
using UnityEngine;

#endregion

namespace Template.Content.Scripts.Card.Fsm.States
{
    /// <summary>
    ///     Truth check, pile award/take, trust hit, win condition check, and turn swapping.
    ///     Main decider for all turn outcomes.
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
                $"[CardRound] Resolve.Enter pile={m_Board.Pile.Count}, challenged={m_Board.LastPlayWasChallenged}, trust={m_Board.TrustTowardPlayer:0.00}");

            if (m_Board.LastPlayWasChallenged)
                ResolveChallenge();

            // Check Win / Loss condition
            if (m_Board.PlayerHand.Count == 0)
            {
                Debug.Log("[CardRound] Player has emptied their hand! Player wins the round!");
                m_Fsm.SetState(null);

                GameManager.Instance.EndDialogue(true);

                //GameManager.Instance.DialogueManager.StartDialogue(GameManager.Instance.m_AIProfile.m_Character.loseDialogue);
                //Debug.Log(GameManager.Instance.m_AIProfile.m_Character.loseDialogue);

                return;
            }

            if (m_Board.OpponentHand.Count == 0)
            {
                Debug.Log($"[CardRound] {m_Board.GetOpponentLabel()} has emptied their hand! Opponent wins the round!");
                m_Fsm.SetState(null);
                GameManager.Instance.EndDialogue(false);
                return;
            }

            // If round continues, swap active seat and return to DecideState
            m_Board.SwapActiveTurn();
            m_Fsm.SetState(new DecideState(m_Fsm, m_Board));
        }

        public void Exit()
        {
        }

        private void ResolveChallenge()
        {
            // Determine if the active user was telling the truth
            var wasBluff = false;
            for (var i = 0; i < m_Board.LastPlayedCards.Count; i++)
            {
                if (m_Board.LastPlayedCards[i]
                    .IsColourOutsideThreshold(m_Board
                        .TargetColour)) // If any card is not within the colour threshold, the active player was bluffing
                {
                    wasBluff = true;
                    break;
                }
            }

            if (wasBluff)
            {
                Debug.Log($"[CardRound] Challenge SUCCEEDED! {m_Board.ActiveTurn} was caught bluffing!");
                if (m_Board.ActiveTurn == TurnUser.Player)
                {
                    GameManager.Instance.AIDialogueChallengeCorrect();

                    // Lost trust as the player was caught
                    m_Board.ShiftTrust(false);
                    // Player was bluffing so they take the pile into their hand
                    m_Board.PlayerHand.AddRange(m_Board.Pile);
                    GameManager.Instance.SpawnPlayerHandItems();
                }
                else
                {
                    GameManager.Instance.ChallengeOpponentCorrect();
                    // Opponent was caught bluffing so they take the pile into their hand
                    m_Board.OpponentHand.AddRange(m_Board.Pile);
                }
            }
            else if (m_Board.ActiveTurn == TurnUser.Player) // Not a bluff
            {
                GameManager.Instance.ChallengeOpponentWrong();

                Debug.Log($"[CardRound] Challenge FAILED! {m_Board.ActiveTurn} was telling the truth!");
                // Player called bluff on opponent but opponent was honest, so player loses trust
                m_Board.PlayerHand.AddRange(m_Board.Pile);
                GameManager.Instance.SpawnPlayerHandItems();
            }
            else
            {
                GameManager.Instance.AIDialogueChallengeWrong();

                Debug.Log($"[CardRound] Challenge FAILED! {m_Board.ActiveTurn} was telling the truth!");
                // Opponent called bluff on player but player was honest, so opponent trusts the player more
                m_Board.ShiftTrust(true);
                m_Board.OpponentHand.AddRange(m_Board.Pile);
            }

            // Clear the pile for the next turn as someone will always take it after a challenge has been initiated
            m_Board.Pile.Clear();
        }
    }
}