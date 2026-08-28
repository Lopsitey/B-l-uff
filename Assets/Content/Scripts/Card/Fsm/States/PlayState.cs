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
        private readonly List<CardID> m_ChosenCards;

        public PlayState(FSM fsm, GameBlackboard board, List<CardID> chosenCards)
        {
            m_ChosenCards = chosenCards;
            m_Fsm = fsm;
            m_Board = board;
        }

        public void Enter()
        {
            var activeHand = m_Board.GetHand(m_Board.ActiveTurn);

            m_Board.LastPlayedCards.Clear();

            for (int i = 0; i < m_ChosenCards.Count; i++)
            {
                // Only cards genuinely in hand may be committed, so a stale UI selection cannot duplicate a card
                if (!activeHand.Remove(m_ChosenCards[i])) continue;

                m_Board.LastPlayedCards.Add(m_ChosenCards[i]);
                m_Board.Pile.Add(m_ChosenCards[i]);
            }

            Debug.Log(
                $"[CardRound] {m_Board.ActiveTurn} committed {m_Board.LastPlayedCards.Count} card(s) claimed as {m_Board.TargetColour}. Pot is now {m_Board.PileSize}.");
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
