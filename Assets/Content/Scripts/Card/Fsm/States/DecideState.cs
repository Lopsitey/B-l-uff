#region

using System.Collections.Generic;
using Template.Content.Scripts.Card.Blackboard;
using Template.Content.Scripts.Card.Data;
using UnityEngine;
using static Template.Content.Scripts.Card.Fuzzy.AIFuzzyBrain;

#endregion

namespace Template.Content.Scripts.Card.Fsm.States
{
    internal sealed class DecideState : ICardRoundState
    {
        private readonly FSM m_Fsm;
        private readonly GameBlackboard m_Board;

        public DecideState(FSM fsm, GameBlackboard board)
        {
            m_Fsm = fsm;
            m_Board = board;
        }

        public void Enter()
        {
            Debug.Log($"[CardRound] Decide.Enter: ActiveTurn = {m_Board.ActiveTurn}");

            if (m_Board.ActiveTurn == TurnUser.Opponent)
            {
                ExecuteAIDecision();
            }
            else
            {
                Debug.Log($"[CardRound] Player's turn! Hand count: {m_Board.PlayerHand.Count}. Press [Space] to play first card, or use UI.");
            }
        }

        public void Tick()
        {
            // Placeholder debug input for testing without full UI
            if (m_Board.ActiveTurn == TurnUser.Player)
            {
                if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
                {
                    PlayFirstAvailableCard();
                }
            }
        }

        public void Exit()
        {
        }

        public void ConfirmDecision(CardColour claimedColour, List<CardID> chosenCards)
        {
            m_Board.TargetColour = claimedColour;
            m_Fsm.SetState(new PlayState(m_Fsm, m_Board, chosenCards));
        }

        private void PlayFirstAvailableCard()
        {
            if (m_Board.PlayerHand.Count == 0) return;

            var chosen = new List<CardID> { m_Board.PlayerHand[0] };
            // Claim current target colour (or the card's own colour if pile is empty)
            var claim = m_Board.PileSize == 0 ? chosen[0].Colour : m_Board.TargetColour;
            ConfirmDecision(claim, chosen);
        }

        private void ExecuteAIDecision()
        {
            if (m_Board.OpponentHand.Count == 0) return;

            const float fuzzyThreshold = 0.65f;
            var isBluffing = EvaluateBluffRisk(m_Board) > fuzzyThreshold;
            var chosenCards = new List<CardID>();
            CardColour claimedColour;

            if (m_Board.PileSize == 0)
            {
                // First play on empty pile: play any card honestly
                var card = m_Board.OpponentHand[0];
                chosenCards.Add(card);
                claimedColour = card.Colour;
            }
            else if (!isBluffing)
            {
                // Honest attempt: look for a matching target colour in hand
                for (int i = 0; i < m_Board.OpponentHand.Count; i++)
                {
                    if (m_Board.OpponentHand[i].Colour == m_Board.TargetColour)
                    {
                        chosenCards.Add(m_Board.OpponentHand[i]);
                        break;
                    }
                }

                if (chosenCards.Count > 0)
                {
                    claimedColour = m_Board.TargetColour;
                }
                else
                {
                    // No matching card found, must bluff with first available card
                    chosenCards.Add(m_Board.OpponentHand[0]);
                    claimedColour = m_Board.TargetColour;
                }
            }
            else
            {
                // Bluff attempt: pick first card and claim current target colour
                chosenCards.Add(m_Board.OpponentHand[0]);
                claimedColour = m_Board.TargetColour;
            }

            ConfirmDecision(claimedColour, chosenCards);
        }
    }
}
