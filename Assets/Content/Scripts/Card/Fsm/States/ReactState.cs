#region

using Template.Content.Scripts.Card.Blackboard;
using Template.Content.Scripts.Card.Data;
using Template.Content.Scripts.Card.Fuzzy;
using UnityEngine;
using UnityEngine.InputSystem;

#endregion

namespace Template.Content.Scripts.Card.Fsm.States
{
    /// <summary>
    ///     Gives the defender (non-active seat) the opportunity to call the bluff or pass.
    /// </summary>
    internal sealed class ReactState : ICardRoundState
    {
        /// <summary>Seconds the AI pauses before reacting, so the play is readable.</summary>
        private const float AiReactDelay = 0.7f;
        private const float CallThreshold = 0.5f;

        private readonly FSM m_Fsm;
        private readonly GameBlackboard m_Board;

        private float m_ReactTimer;

        public ReactState(FSM fsm, GameBlackboard board)
        {
            m_Fsm = fsm;
            m_Board = board;
        }

        /// <summary>True while the player is the one who must choose to call or pass.</summary>
        public bool AwaitingPlayerReaction => m_Board.ActiveTurn == TurnUser.Opponent;

        public void Enter()
        {
            m_ReactTimer = 0f;
            Debug.Log($"[CardRound] React.Enter: defending against {m_Board.ActiveTurn}'s play.");

            if (AwaitingPlayerReaction)
            {
                Debug.Log(
                    $"[CardRound] Opponent claims {m_Board.TargetColour}. Press [C] to challenge or [P] to pass.");
            }
        }

        public void Tick()
        {
            if (AwaitingPlayerReaction)
            {
                var keyboard = Keyboard.current;
                if (keyboard == null) return;

                if (keyboard[Key.C].wasPressedThisFrame)
                {
                    Respond(true);
                }
                else if (keyboard[Key.P].wasPressedThisFrame || keyboard[Key.Space].wasPressedThisFrame)
                {
                    Respond(false);
                }

                return;
            }

            // Player played, so the AI is the defender
            m_ReactTimer += Time.deltaTime;
            if (m_ReactTimer < AiReactDelay) return;

            var urge = AIFuzzyBrain.EvaluateCallChance(m_Board);
            Debug.Log($"[CardRound] Opponent call urge = {urge:0.00} (threshold {CallThreshold:0.00})");
            Respond(urge >= CallThreshold);
        }

        public void Exit()
        {
        }

        /// <summary>
        ///     Single exit point for both the player (UI) and the AI reaction.
        /// </summary>
        public void Respond(bool challenge)
        {
            Debug.Log(challenge
                ? $"[CardRound] {(AwaitingPlayerReaction ? "Player" : "Opponent")} CHALLENGES the play."
                : $"[CardRound] {(AwaitingPlayerReaction ? "Player" : "Opponent")} passes.");

            m_Board.LastPlayWasChallenged = challenge;
            m_Fsm.SetState(new ResolveState(m_Fsm, m_Board));
        }
    }
}
