#region

using System.Collections.Generic;
using Template.Content.Scripts.Card.Blackboard;
using Template.Content.Scripts.Card.Data;
using UnityEngine;
using UnityEngine.InputSystem;
using static Template.Content.Scripts.Card.Fuzzy.AIFuzzyBrain;

#endregion

namespace Template.Content.Scripts.Card.Fsm.States
{
    /// <summary>
    ///     The active seat picks cards and the colour it claims they are. Nothing leaves a hand here.
    /// </summary>
    internal sealed class DecideState : ICardRoundState
    {
        /// <summary>Seconds the AI pauses before committing, so the table stays readable.</summary>
        private const float AiThinkDelay = 0.8f;

        private const float BluffThreshold = 0.62f;
        private const float ComboThreshold = 0.5f;
        private const int MaxCardsPerPlay = 3;

        private readonly FSM m_Fsm;
        private readonly GameBlackboard m_Board;

        private float m_ThinkTimer;

        public DecideState(FSM fsm, GameBlackboard board)
        {
            m_Fsm = fsm;
            m_Board = board;
        }

        /// <summary>True while the player is the one choosing cards.</summary>
        public bool AwaitingPlayerChoice => m_Board.ActiveTurn == TurnUser.Player;

        public void Enter()
        {
            m_ThinkTimer = 0f;

            if (AwaitingPlayerChoice)
            {
                Debug.Log(m_Board.PileIsOpen
                    ? "[CardRound] Your turn. The pot is open, so claim any colour."
                    : $"[CardRound] Your turn. Pot claims {m_Board.TargetColour}, so you may claim {m_Board.TargetColour.Step(-1)}, {m_Board.TargetColour} or {m_Board.TargetColour.Step(1)}.");
            }
        }

        public void Tick()
        {
            if (!AwaitingPlayerChoice)
            {
                m_ThinkTimer += Time.deltaTime;
                if (m_ThinkTimer >= AiThinkDelay) ExecuteAIDecision();
                return;
            }

            // Quick-play shortcut: first card in hand, claiming the colour already on the pot
            var keyboard = Keyboard.current;
            if (keyboard != null && keyboard[Key.Space].wasPressedThisFrame && m_Board.PlayerHand.Count > 0)
            {
                var card = m_Board.PlayerHand[0];
                ConfirmDecision(m_Board.PileIsOpen ? card.Colour : m_Board.TargetColour, new List<CardID> { card });
            }
        }

        public void Exit()
        {
        }

        /// <summary>
        ///     Single exit point for both the player (UI) and the AI once cards and a claim are chosen.
        /// </summary>
        public void ConfirmDecision(CardColour claimedColour, List<CardID> chosenCards)
        {
            if (chosenCards == null || chosenCards.Count == 0) return;

            if (!m_Board.IsLegalClaim(claimedColour))
            {
                Debug.LogWarning(
                    $"[CardRound] Rejected claim {claimedColour}: must be within one step of {m_Board.TargetColour}.");
                return;
            }

            m_Board.TargetColour = claimedColour;
            m_Fsm.SetState(new PlayState(m_Fsm, m_Board, chosenCards));
        }

        private void ExecuteAIDecision()
        {
            var hand = m_Board.OpponentHand;
            if (hand.Count == 0) return;

            var legalClaims = m_Board.GetLegalClaims();
            var isBluffing = EvaluateBluffRisk(m_Board) > BluffThreshold;

            var claim = ChooseClaim(legalClaims, isBluffing, out var matchesForClaim);

            // Falling back to honesty when the chosen claim happens to be covered keeps the lie meaningful
            var playingHonestly = matchesForClaim > 0 && !isBluffing;
            var comboStrength = Mathf.Clamp01(matchesForClaim / 3f);
            var wantsCombo = EvaluateComboAppetite(m_Board, comboStrength, !playingHonestly) > ComboThreshold;
            var cardsWanted = wantsCombo ? 2 : 1;

            var chosen = playingHonestly
                ? TakeMatching(claim, Mathf.Min(cardsWanted, matchesForClaim))
                : TakeJunk(legalClaims, Mathf.Min(cardsWanted, MaxCardsPerPlay));

            Debug.Log(
                $"[CardRound] Opponent claims {claim} with {chosen.Count} card(s) ({(playingHonestly ? "honest" : "bluffing")}).");
            ConfirmDecision(claim, chosen);
        }

        /// <summary>
        ///     Scores each legal claim. Honest play favours colours it actually holds, bluffs favour colours it
        ///     does not, and both nudge away from simply echoing the colour already on the pot.
        /// </summary>
        private CardColour ChooseClaim(CardColour[] legalClaims, bool isBluffing, out int matchesForClaim)
        {
            var bestClaim = legalClaims[0];
            var bestScore = float.MinValue;
            var bestMatches = 0;

            for (int i = 0; i < legalClaims.Length; i++)
            {
                var candidate = legalClaims[i];
                var matches = m_Board.CountColourInHand(TurnUser.Opponent, candidate);

                var score = isBluffing ? (matches == 0 ? 10f : 0f) : matches * 10f;
                if (!m_Board.PileIsOpen && candidate != m_Board.TargetColour) score += 3f;
                score += Random.value;

                if (score <= bestScore) continue;

                bestScore = score;
                bestClaim = candidate;
                bestMatches = matches;
            }

            matchesForClaim = bestMatches;
            return bestClaim;
        }

        private List<CardID> TakeMatching(CardColour colour, int count)
        {
            var chosen = new List<CardID>(count);
            var hand = m_Board.OpponentHand;

            for (int i = 0; i < hand.Count && chosen.Count < count; i++)
                if (hand[i].Colour == colour)
                    chosen.Add(hand[i]);

            return chosen;
        }

        /// <summary>Dumps cards that no legal claim could ever cover, keeping useful colours in hand.</summary>
        private List<CardID> TakeJunk(CardColour[] legalClaims, int count)
        {
            var chosen = new List<CardID>(count);
            var hand = m_Board.OpponentHand;

            for (int i = 0; i < hand.Count && chosen.Count < count; i++)
            {
                var isUseful = false;
                for (int c = 0; c < legalClaims.Length; c++)
                    if (hand[i].Colour == legalClaims[c])
                    {
                        isUseful = true;
                        break;
                    }

                if (!isUseful) chosen.Add(hand[i]);
            }

            // Every card was useful, so give up the first one rather than stalling
            if (chosen.Count == 0) chosen.Add(hand[0]);

            return chosen;
        }
    }
}
