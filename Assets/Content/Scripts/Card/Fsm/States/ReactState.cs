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
            Debug.Log($"[CardRound] React.Enter: Defender against {m_Board.ActiveTurn}'s play.");

            if (m_Board.ActiveTurn == TurnUser.Player)
            {
                // Player just played -> AI is the defender reacting
                var urge = AIFuzzyBrain.EvaluateCallChance(m_Board);
                const float callThreshold = 0.5f;
                m_Board.LastPlayWasChallenged = urge >= callThreshold;

                Debug.Log($"[CardRound] Opponent reaction: call urge = {urge:0.00}, challenged = {m_Board.LastPlayWasChallenged}");
                m_Fsm.SetState(new ResolveState(m_Fsm, m_Board));
            }
            else
            {
                // Opponent just played -> Player is the defender reacting
                Debug.Log("[CardRound] Opponent played cards! Press [C] to Challenge / Call Cheat, or [P]/[Space] to Pass.");
            }
        }

        public void Tick()
        {
            // Placeholder debug input when Player is the defender
            if (m_Board.ActiveTurn == TurnUser.Opponent)
            {
                if (UnityEngine.Input.GetKeyDown(KeyCode.C))
                {
                    Debug.Log("[CardRound] Player chose to CHALLENGE!");
                    m_Board.LastPlayWasChallenged = true;
                    m_Fsm.SetState(new ResolveState(m_Fsm, m_Board));
                }
                else if (UnityEngine.Input.GetKeyDown(KeyCode.P) || UnityEngine.Input.GetKeyDown(KeyCode.Space))
                {
                    Debug.Log("[CardRound] Player chose to PASS.");
                    m_Board.LastPlayWasChallenged = false;
                    m_Fsm.SetState(new ResolveState(m_Fsm, m_Board));
                }
            }
        }

        public void Exit()
        {
        }

        public void Respond(bool challenge)
        {
            m_Board.LastPlayWasChallenged = challenge;
            m_Fsm.SetState(new ResolveState(m_Fsm, m_Board));
        }
    }
}
