#region

using System.Collections.Generic;
using Template.Content.Scripts.Card.Blackboard;
using Template.Content.Scripts.Card.Data;
using UnityEngine;
using static Template.Content.Scripts.Card.Fuzzy.AIFuzzyBrainUtil;

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
            if (m_Board.ActiveTurn == TurnUser.Player)
            {
                Debug.Log($"[CardRound] Decide.Enter seat={m_Board.ActiveTurn} player vs {m_Board.GetOpponentLabel()}");
            }
            else
            {
                Debug.Log(
                    $"[CardRound] Decide.Enter seat={m_Board.ActiveTurn} opponent vs {m_Board.GetOpponentLabel()}");
                const float fuzzyThreshold = 0.85f;
                const float comboThreshold = 0.5f;
                Debug.Log($"[CardRound] Decide.Enter seat={m_Board.ActiveTurn}");

                if (EvaluateBluffRisk(m_Board) > fuzzyThreshold)
                {
                    Debug.Log($"[CardRound] bluff risk = {EvaluateBluffRisk(m_Board):0.00}");

                    var bluffCombo = EvaluateComboAppetite(m_Board, 0f, true);
                    Debug.Log($"[CardRound] combo appetite honest={bluffCombo:0.00} bluff={bluffCombo:0.00}");

                    if (bluffCombo > comboThreshold)
                    {
                        Debug.Log($"[CardRound] combo appetite = {bluffCombo:0.00}");
                    }
                    else
                    {
                        // TODO: Implement AI bluff decision logic.
                        ConfirmDecision(m_Board.OpponentHand[0].Colour, m_Board.OpponentHand.GetRange(0, 3));
                    }
                }
                else
                {
                    var honestCombo = EvaluateComboAppetite(m_Board, 0f, false);
                    Debug.Log($"[CardRound] combo appetite honest={honestCombo:0.00} bluff={honestCombo:0.00}");

                    if (honestCombo > comboThreshold)
                    {
                        Debug.Log($"[CardRound] combo appetite = {honestCombo:0.00}");
                    }
                    else
                    {
                        //TODO: Make AI pick more interesting cards to play than just the first 3 in their hand. For now, just pick the first 3 cards in their hand.
                        ConfirmDecision(m_Board.OpponentHand[0].Colour, m_Board.OpponentHand.GetRange(0, 3));
                    }
                }
            }
        }

        // Called when decision is ready (by AI in Enter or by Player UI button callback)
        // Once cards + claim are decided, transition to PlayState
        public void ConfirmDecision(CardColour claimedColour, List<CardID> chosenCards)
        {
            m_Board.TargetColour = claimedColour;
            // Clean transition into PlayState
            m_Fsm.SetState(new PlayState(m_Fsm, m_Board, chosenCards));
        }

        public void Exit()
        {
        }
    }
}