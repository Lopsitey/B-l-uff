#region

using Template.Content.Scripts.Card.Blackboard;
using Template.Content.Scripts.Card.Data;
using Template.Content.Scripts.Card.Fuzzy;
using UnityEngine;

#endregion

namespace Template.Content.Scripts.Card.Fsm.States
{
    /// <summary>
    ///     Gives the defender (non-active seat) the opportunity to call the bluff or pass.
    /// </summary>
    internal sealed class ReactState : ICardRoundState
    {
        private readonly FSM m_Fsm;
        private readonly GameBlackboard m_Board;

        public ReactState(FSM fsm, GameBlackboard board)
        {
            m_Fsm = fsm;
            m_Board = board;
        }

        public void Enter()
        {
            Debug.Log($"[CardRound] React.Enter defender vs {m_Board.GetOpponentLabel()}");

            if (m_Board.ActiveTurn == TurnUser.Player)
            {
                Debug.Log($"[CardRound] call chance = {AIFuzzyBrain.EvaluateCallChance(m_Board):0.00}");
                // Player played: opponent reacts using fuzzy evaluation
                var urge = AIFuzzyBrain.EvaluateCallChance(m_Board);
                const float callThreshold = 0.5f;
                m_Board.LastPlayWasChallenged = urge >= callThreshold;
                Debug.Log(
                    $"[CardRound] opponent call urge = {urge:0.00}, challenged = {m_Board.LastPlayWasChallenged}");
            }
            else
            {
                // Opponent played: player reacts (defaulting to false until UI hook connected)
                m_Board.LastPlayWasChallenged = false;
            }
        }

        public void Tick()
        {
            // Option B: Always transition to ResolveState to evaluate turn outcome centrally
            m_Fsm.SetState(new ResolveState(m_Fsm, m_Board));
        }

        public void Exit()
        {
        }
    }
}