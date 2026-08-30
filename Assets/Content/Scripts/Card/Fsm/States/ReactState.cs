#region

using System.Collections;
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
        private Coroutine m_ReactCoroutine;

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
                if (GameManager.Instance != null)
                    m_ReactCoroutine = GameManager.Instance.StartCoroutine(OpponentReactRoutine());
            }
            else
            {
                // Player reacts to opponent (currently trying to play card/s)
                Debug.Log($"Player vs {m_Board.GetOpponentLabel()}");
                m_Board.LastPlayWasChallenged = false;
            }
        }

        private IEnumerator OpponentReactRoutine()
        {
            yield return new WaitForSeconds(0.4f);

            var dialogueMgr = GameManager.Instance != null ? GameManager.Instance.m_DialogueManager : null;
            while (dialogueMgr != null && dialogueMgr.IsDialogueActive)
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.4f);

            var urge = AIFuzzyBrainUtil.EvaluateCallChance(m_Board);
            const float callThreshold = 0.5f;
            m_Board.LastPlayWasChallenged = urge >= callThreshold;

            if (m_Board.LastPlayWasChallenged)
            {
                Debug.Log(
                    $"[CardRound] opponent call urge = {urge:0.00}, challenged = {m_Board.LastPlayWasChallenged}");
                if (GameManager.Instance != null && GameManager.Instance.m_OpponentArmManager != null)
                    GameManager.Instance.m_OpponentArmManager.RevealItem();
            }
            else
            {
                Debug.Log(
                    $"[CardRound] opponent call urge = {urge:0.00}, {m_Board.GetOpponentLabel()} passed on Player's play");
                if (GameManager.Instance != null && GameManager.Instance.m_OpponentArmManager != null)
                    GameManager.Instance.m_OpponentArmManager.DropItem();
            }

            yield return new WaitForSeconds(0.3f);

            m_Fsm.SetState(new ResolveState(m_Fsm, m_Board));
        }

        public void Challenge()
        {
            Debug.Log($"[CardRound] React.Challenge");
            m_Board.LastPlayWasChallenged = true;
            m_Fsm.SetState(new ResolveState(m_Fsm, m_Board));

            if (GameManager.Instance != null && GameManager.Instance.m_PlayerArmManager != null)
                GameManager.Instance.m_PlayerArmManager.RevealItem();
        }

        public void Pass()
        {
            Debug.Log($"[CardRound] React.Pass");

            m_Board.LastPlayWasChallenged = false;
            m_Fsm.SetState(new ResolveState(m_Fsm, m_Board));

            if (GameManager.Instance != null && GameManager.Instance.m_PlayerArmManager != null)
                GameManager.Instance.m_PlayerArmManager.DropItem();
        }

        public void Exit()
        {
            if (m_ReactCoroutine != null && GameManager.Instance != null)
            {
                GameManager.Instance.StopCoroutine(m_ReactCoroutine);
                m_ReactCoroutine = null;
            }
        }
    }
}