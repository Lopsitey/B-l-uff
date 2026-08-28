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
        /// <summary>Extra cards a caught liar draws from outside play, on top of taking the pot.</summary>
        private const int PenaltyDraw = 2;

        /// <summary>Seconds a revealed challenge stays on screen before the next turn begins.</summary>
        private const float RevealHold = 1.6f;

        private readonly FSM m_Fsm;
        private readonly GameBlackboard m_Board;

        private float m_HoldTimer;

        public ResolveState(FSM fsm, GameBlackboard board)
        {
            m_Fsm = fsm;
            m_Board = board;
        }

        /// <summary>What the challenge exposed, for the UI to show while the reveal is held.</summary>
        public string RevealSummary { get; private set; }

        public void Enter()
        {
            m_HoldTimer = 0f;
            RevealSummary = null;

            if (m_Board.LastPlayWasChallenged)
            {
                ResolveChallenge();
            }
        }

        public void Tick()
        {
            // A revealed challenge is the one moment the player learns something, so hold it long enough to read
            if (RevealSummary != null)
            {
                m_HoldTimer += Time.deltaTime;
                if (m_HoldTimer < RevealHold) return;
            }

            // Win checks run after the challenge so a caught liar cannot win on the play that exposed them
            if (m_Board.PlayerHand.Count == 0)
            {
                m_Board.RoundWinner = TurnUser.Player;
                Debug.Log("[CardRound] *** PLAYER WINS THE ROUND! (Hand is empty) ***");
                m_Fsm.SetState(null);
                return;
            }

            if (m_Board.OpponentHand.Count == 0)
            {
                m_Board.RoundWinner = TurnUser.Opponent;
                Debug.Log($"[CardRound] *** {m_Board.GetOpponentLabel()} WINS THE ROUND! (Hand is empty) ***");
                m_Fsm.SetState(null);
                return;
            }

            m_Board.SwapActiveTurn();
            m_Fsm.SetState(new DecideState(m_Fsm, m_Board));
        }

        public void Exit()
        {
        }

        private void ResolveChallenge()
        {
            var activeSeat = m_Board.ActiveTurn;
            var defendingSeat = m_Board.DefendingSeat;

            var wasBluff = false;
            for (var i = 0; i < m_Board.LastPlayedCards.Count; i++)
            {
                if (m_Board.LastPlayedCards[i].Colour != m_Board.TargetColour)
                {
                    wasBluff = true;
                    break;
                }
            }

            m_Board.RecordReveal(activeSeat, wasBluff);
            RevealSummary = BuildRevealSummary(activeSeat, defendingSeat, wasBluff);
            Debug.Log($"[CardRound] {RevealSummary}");

            if (wasBluff)
            {
                if (m_Board.LastPlayedCards.Count > 0)
                {
                    Debuffs.ApplyCardDebuff(m_Board.LastPlayedCards[0]);
                }

                // A caught liar eats the pot plus cards from outside play, so lying costs more than a bad call
                m_Board.TakePile(activeSeat);
                m_Board.DrawExtraCards(activeSeat, PenaltyDraw);
                return;
            }

            m_Board.TakePile(defendingSeat);
        }

        /// <summary>Spells out the revealed cards so the player can see exactly what they called.</summary>
        private string BuildRevealSummary(TurnUser activeSeat, TurnUser defendingSeat, bool wasBluff)
        {
            var revealed = string.Empty;
            for (var i = 0; i < m_Board.LastPlayedCards.Count; i++)
            {
                revealed += (i > 0 ? ", " : string.Empty) + m_Board.LastPlayedCards[i].Colour;
            }

            return wasBluff
                ? $"Caught! {activeSeat} claimed {m_Board.TargetColour} but played {revealed}. {activeSeat} takes the pot of {m_Board.PileSize} plus {PenaltyDraw} more."
                : $"Honest. {activeSeat} really had {revealed}. {defendingSeat} takes the pot of {m_Board.PileSize}.";
        }
    }
}
