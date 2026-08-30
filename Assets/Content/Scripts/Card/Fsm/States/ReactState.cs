#region

using Template.Content.Scripts.Card.Blackboard;
using Template.Content.Scripts.Card.Data;
using Template.Content.Scripts.Card.Fuzzy;
using Template.Content.Scripts.Managers;
using UnityEngine;

#endregion

namespace Template.Content.Scripts.Card.Fsm.States
{
    /// <summary>
    ///     Handles interaction between the active player and the idle player.
    ///     Gives the idle player the opportunity to call the bluff or pass.
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
            Debug.Log($"[CardRound] React.Enter");

            if (m_Board.ActiveTurn == TurnUser.Player) // Opponent reacts to player (currently trying to play card/s)
            {
                Debug.Log($"Opponent vs {m_Board.GetOpponentLabel()}");

                // Player played: opponent reacts using fuzzy evaluation
                Debug.Log($"[CardRound] call chance = {AIFuzzyBrainUtil.EvaluateCallChance(m_Board):0.00}");
                var urge = AIFuzzyBrainUtil.EvaluateCallChance(m_Board);
                const float callThreshold = 0.5f;
                m_Board.LastPlayWasChallenged = urge >= callThreshold;

                if (m_Board.LastPlayWasChallenged)
                {
                    Debug.Log(
                        $"[CardRound] opponent call urge = {urge:0.00}, challenged = {m_Board.LastPlayWasChallenged}");
                    GameManager.Instance.AIDialogueChallenge();
                }
                else
                    Debug.Log(
                        $"[CardRound] opponent call urge = {urge:0.00}, {m_Board.GetOpponentLabel()} passed on Player's play");

                m_Fsm.SetState(new ResolveState(m_Fsm, m_Board));
            }
            else
            {
                // Player reacts to opponent (currently trying to play card/s)
                Debug.Log($"Player vs {m_Board.GetOpponentLabel()}");
                m_Board.LastPlayWasChallenged = false;
            }
        }

        public void Challenge()
        {
            Debug.Log($"[CardRound] React.Challenge");
            m_Board.LastPlayWasChallenged = true;
            m_Fsm.SetState(new ResolveState(m_Fsm, m_Board));

            GameManager.Instance.m_PlayerArmManager.RevealItem();
        }

        public void Pass()
        {
            Debug.Log($"[CardRound] React.Pass");

            m_Board.LastPlayWasChallenged = false;
            m_Fsm.SetState(new ResolveState(m_Fsm, m_Board));

            GameManager.Instance.m_PlayerArmManager.DropItem();
        }

        public void Exit()
        {
        }
    }
}