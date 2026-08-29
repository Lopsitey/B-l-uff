#region

using System.Collections.Generic;
using Template.Content.Scripts.Card.Blackboard;
using Template.Content.Scripts.Card.Data;
using UnityEngine;

#endregion

namespace Template.Content.Scripts.Card.Fsm.States
{
    /// <summary>
    ///     Commit cards to the pile and announce the claim.
    /// </summary>
    internal sealed class PlayState : ICardRoundState
    {
        private readonly FSM m_Fsm;
        private readonly GameBlackboard m_Board;

        private List<CardID> m_ChosenCards;

        public PlayState(FSM fsm, GameBlackboard board, List<CardID> chosenCards)
        {
            m_ChosenCards = chosenCards;
            m_Fsm = fsm;
            m_Board = board;
        }

        public void Enter()
        {
            Debug.Log($"[CardRound] Play.Enter pile={m_Board.Pile.Count}");

            var activeHand = m_Board.ActiveTurn == TurnUser.Player ? m_Board.PlayerHand : m_Board.OpponentHand;

            //Clear the prior turn's cards
            m_Board.LastPlayedCards.Clear();
            foreach (var t in m_ChosenCards)
                activeHand.Remove(t);

            // Stage the chosen cards onto the blackboard for the react/resolve states to use
            m_Board.LastPlayedCards.AddRange(m_ChosenCards);

            // Commit the chosen cards to the pile
            m_Board.Pile.AddRange(m_ChosenCards);

            if (m_Board.ActiveTurn == TurnUser.Opponent)
            {
                Debug.Log(
                    $"[CardRound] Opponent played {m_ChosenCards.Count} card/s of claimed colour {m_Board.TargetColour}");
                m_Fsm.SetState(new ReactState(m_Fsm, m_Board));
            }
        }

        // Called when the player has finished playing the cards (e.g. animation finished).
        // Transitions to ReactState to allow the AI to react.
        public void CompletePlay() => m_Fsm.SetState(new ReactState(m_Fsm, m_Board));

        public void Exit()
        {
        }
    }
}