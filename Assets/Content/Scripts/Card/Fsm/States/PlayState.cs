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
            Debug.Log($"[CardRound] Play.Enter pile={m_Board.PileSize}");

            var activeHand = m_Board.ActiveTurn == TurnUser.Player ? m_Board.PlayerHand : m_Board.OpponentHand;

            //Clear the prior turn's cards
            m_Board.LastPlayedCards.Clear();
            /*TODO for (int i = 0; i < m_ChosenCards.Count; i++)
                        activeHand.Remove(m_ChosenCards[i]);*/

            // Stage the chosen cards onto the blackboard
            m_Board.LastPlayedCards.AddRange(m_ChosenCards);

            // Update the pile size
            m_Board.PileSize += m_Board.LastPlayedCards.Count;
        }

        public void Tick()
        {
            // After cards are committed face-down to the pot, move to ReactState (Await Challenge)
            m_Fsm.SetState(new ReactState(m_Fsm, m_Board));
        }

        public void Exit()
        {
        }
    }
}