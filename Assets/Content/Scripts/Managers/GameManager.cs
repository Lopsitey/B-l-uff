#region

using System.Collections.Generic;
using Template.Content.Scripts.Card.Blackboard;
using Template.Content.Scripts.Card.Data;
using Template.Content.Scripts.Card.Fsm;
using Template.Content.Scripts.Card.Fsm.States;
using Template.Content.Scripts.Card.Fuzzy;
using UnityEngine;
using UnityEngine.Serialization;

#endregion

namespace Template.Content.Scripts.Managers
{
    /// <summary>
    ///     The stub that starts everything off. A small manager mainly used for its MonoBehaviour.
    ///     The real game management is in the FSM and blackboard. This manager is a singleton so that other systems can access
    ///     the blackboard and FSM.
    /// </summary>
    public sealed class GameManager : Singleton<GameManager>
    {
        [Tooltip("Required. Fuzzy profile for the god you face this round.")]
        [FormerlySerializedAs("m_OpponentProfile")]
        [SerializeField]
        private AIFuzzyProfile m_AIProfile;

        [Tooltip("Draws a minimal debug UI in the Game view for testing without full UI setup.")]
        [SerializeField]
        private bool m_ShowDebugOverlay = true;

        private GameBlackboard m_Blackboard;
        private AIFuzzyBrain m_Fuzzy;
        private FSM m_Fsm;

        internal GameBlackboard Blackboard => m_Blackboard;
        internal FSM Fsm => m_Fsm;

        private void Start()
        {
            if (!HasOpponentProfile()) return;

            BeginRound();
        }

        private void Update() => m_Fsm?.Tick();

        private void BeginRound()
        {
            // Instantiates the round blackboard, FSM and AI
            m_Fuzzy = new AIFuzzyBrain();
            m_Blackboard = new GameBlackboard(m_AIProfile);
            m_Fsm = new FSM(m_Blackboard, m_Fuzzy);
            m_Fsm.SetState(new DecideState(m_Fsm, m_Blackboard));
            Debug.Log($"[GameManager] Round started vs {m_AIProfile.DisplayName}.");
        }

        public void ResetRound()
        {
            if (m_AIProfile != null)
                BeginRound();
        }

        private bool HasOpponentProfile()
        {
            if (m_AIProfile != null)
                return true;

            Debug.LogError("[GameManager] AIFuzzyProfile is required. Assign one on GameManager.", this);
            enabled = false;
            return false;
        }

        private void OnGUI()
        {
            if (!m_ShowDebugOverlay || m_Blackboard == null) return;

            const int panelWidth = 320;
            const int panelHeight = 270;
            GUILayout.BeginArea(new Rect(10, 10, panelWidth, panelHeight), GUI.skin.box);

            GUILayout.Label($"<b>Round vs:</b> {m_Blackboard.GetOpponentLabel()}");
            GUILayout.Label($"<b>Active Turn:</b> {m_Blackboard.ActiveTurn}");
            GUILayout.Label($"<b>Current Target Colour:</b> {m_Blackboard.TargetColour}");
            GUILayout.Label($"<b>Pot / Pile Size:</b> {m_Blackboard.PileSize}");
            GUILayout.Label($"<b>Trust toward Player:</b> {m_Blackboard.TrustTowardPlayer:0.00}");
            GUILayout.Label($"<b>Draw Stack Remaining:</b> {m_Blackboard.DiscardHistory?.Count ?? 0}");
            GUILayout.Label($"<b>Player Hand ({m_Blackboard.PlayerHand.Count}):</b>");

            // Display player's cards
            GUILayout.BeginHorizontal();
            var maxCardsToShow = Mathf.Min(6, m_Blackboard.PlayerHand.Count);
            for (int i = 0; i < maxCardsToShow; i++)
            {
                var card = m_Blackboard.PlayerHand[i];
                if (GUILayout.Button($"{card.Colour}\n{card.Suit}", GUILayout.Width(46), GUILayout.Height(40)))
                {
                    if (m_Blackboard.ActiveTurn == TurnUser.Player)
                    {
                        var chosen = new List<CardID> { card };
                        var claim = m_Blackboard.PileSize == 0 ? card.Colour : m_Blackboard.TargetColour;
                        m_Fsm.SetState(new PlayState(m_Fsm, m_Blackboard, chosen));
                    }
                }
            }
            if (m_Blackboard.PlayerHand.Count > 6)
            {
                GUILayout.Label($"+{m_Blackboard.PlayerHand.Count - 6} more");
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.Label($"<b>Opponent Hand Count:</b> {m_Blackboard.OpponentHand.Count}");

            if (GUILayout.Button("Restart Round"))
            {
                ResetRound();
            }

            GUILayout.EndArea();
        }
    }
}
