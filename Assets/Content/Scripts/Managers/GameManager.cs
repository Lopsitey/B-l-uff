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
        private const int CardsPerRow = 4;

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

        private readonly List<CardID> m_Selection = new List<CardID>(4);
        private CardColour m_PendingClaim;
        private ICardRoundState m_LastSeenState;

        internal GameBlackboard Blackboard => m_Blackboard;
        internal FSM Fsm => m_Fsm;

        private void Start()
        {
            if (!HasOpponentProfile()) return;

            BeginRound();
        }

        private void Update()
        {
            m_Fsm?.Tick();

            // A selection only makes sense for the play being built, so drop it whenever the phase moves on
            if (m_Fsm?.Current == m_LastSeenState) return;

            m_LastSeenState = m_Fsm?.Current;
            m_Selection.Clear();
            if (m_Blackboard != null) m_PendingClaim = m_Blackboard.TargetColour;
        }

        private void BeginRound()
        {
            // Instantiates the round blackboard, FSM and AI
            m_Fuzzy = new AIFuzzyBrain();
            m_Blackboard = new GameBlackboard(m_AIProfile);
            m_Fsm = new FSM(m_Blackboard, m_Fuzzy);
            m_Selection.Clear();
            m_PendingClaim = m_Blackboard.TargetColour;
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

        // A debug overlay I found online because I CBA
        private void OnGUI()
        {
            if (!m_ShowDebugOverlay || m_Blackboard == null) return;

            var label = new GUIStyle(GUI.skin.label) { richText = true };

            GUILayout.BeginArea(new Rect(10, 10, 420, 480), GUI.skin.box);

            GUILayout.Label($"<b>Round vs:</b> {m_Blackboard.GetOpponentLabel()}", label);
            GUILayout.Label($"<b>Active Turn:</b> {m_Blackboard.ActiveTurn}", label);
            GUILayout.Label(
                m_Blackboard.PileIsOpen
                    ? "<b>Claim on pot:</b> none, the pot is open"
                    : $"<b>Claim on pot:</b> {m_Blackboard.TargetColour}  (playable: {m_Blackboard.TargetColour.Step(-1)} / {m_Blackboard.TargetColour} / {m_Blackboard.TargetColour.Step(1)})",
                label);
            GUILayout.Label($"<b>Pot:</b> {m_Blackboard.PileSize} cards", label);
            GUILayout.Label($"<b>Trust toward Player:</b> {m_Blackboard.TrustTowardPlayer:0.00}", label);
            GUILayout.Label($"<b>Penalty Stack:</b> {m_Blackboard.DrawStack?.Count ?? 0}", label);
            GUILayout.Label($"<b>Opponent Hand:</b> {m_Blackboard.OpponentHand.Count} cards", label);
            GUILayout.Label($"<b>Opponent caught lying:</b> {m_Blackboard.OpponentBluffRateObserved:P0}", label);

            GUILayout.Space(4);
            DrawPhaseControls(label);

            GUILayout.Space(4);
            GUILayout.Label("Keys: [Space] quick play / pass, [C] challenge, [P] pass", label);

            if (GUILayout.Button("Restart Round"))
            {
                ResetRound();
            }

            GUILayout.EndArea();
        }

        /// <summary>
        ///     Routes overlay clicks through the same entry points the keyboard uses, so both paths behave identically.
        /// </summary>
        private void DrawPhaseControls(GUIStyle label)
        {
            switch (m_Fsm?.Current)
            {
                case DecideState decide when decide.AwaitingPlayerChoice:
                    DrawClaimPicker(label);
                    DrawHandPicker(label);
                    DrawPlayButton(decide);
                    break;

                case ReactState react when react.AwaitingPlayerReaction:
                    GUILayout.Label(
                        $"<b>Opponent played {m_Blackboard.LastPlayedCards.Count} card(s) claiming {m_Blackboard.TargetColour}. Believe them?</b>",
                        label);
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Challenge (C)", GUILayout.Height(30)))
                    {
                        react.Respond(true);
                    }

                    if (GUILayout.Button("Pass (P)", GUILayout.Height(30)))
                    {
                        react.Respond(false);
                    }

                    GUILayout.EndHorizontal();
                    break;

                case ResolveState resolve when resolve.RevealSummary != null:
                    GUILayout.Label($"<b>{resolve.RevealSummary}</b>", label);
                    break;

                case null:
                    GUILayout.Label(
                        m_Blackboard.RoundWinner == TurnUser.Player
                            ? "<b>You won the round.</b> Press Restart Round."
                            : $"<b>{m_Blackboard.GetOpponentLabel()} won the round.</b> Press Restart Round.",
                        label);
                    break;

                default:
                    GUILayout.Label("<b>Opponent is thinking...</b>", label);
                    break;
            }
        }

        /// <summary>Only legal claims are offered, so the wheel rule cannot be broken from the UI.</summary>
        private void DrawClaimPicker(GUIStyle label)
        {
            GUILayout.Label("<b>Claim:</b>", label);

            var claims = m_Blackboard.GetLegalClaims();
            for (int i = 0; i < claims.Length; i++)
            {
                if (i % CardsPerRow == 0) GUILayout.BeginHorizontal();

                var isPicked = claims[i] == m_PendingClaim;
                if (GUILayout.Button(isPicked ? $"[{claims[i]}]" : claims[i].ToString(), GUILayout.Height(26)))
                {
                    m_PendingClaim = claims[i];
                }

                var isRowEnd = i % CardsPerRow == CardsPerRow - 1 || i == claims.Length - 1;
                if (isRowEnd) GUILayout.EndHorizontal();
            }

            if (!m_Blackboard.IsLegalClaim(m_PendingClaim)) m_PendingClaim = claims[0];
        }

        private void DrawHandPicker(GUIStyle label)
        {
            var hand = m_Blackboard.PlayerHand;
            GUILayout.Label($"<b>Your hand ({hand.Count})</b> - click to select:", label);

            for (int i = 0; i < hand.Count; i++)
            {
                if (i % CardsPerRow == 0) GUILayout.BeginHorizontal();

                var card = hand[i];
                var isSelected = m_Selection.Contains(card);
                var caption = isSelected ? $"* {card.Colour}\n{card.Suit}" : $"{card.Colour}\n{card.Suit}";

                if (GUILayout.Button(caption, GUILayout.Width(92), GUILayout.Height(40)))
                {
                    if (isSelected) m_Selection.Remove(card);
                    else m_Selection.Add(card);
                }

                var isRowEnd = i % CardsPerRow == CardsPerRow - 1 || i == hand.Count - 1;
                if (isRowEnd) GUILayout.EndHorizontal();
            }
        }

        private void DrawPlayButton(DecideState decide)
        {
            var canPlay = m_Selection.Count > 0;
            GUI.enabled = canPlay;

            var caption = canPlay
                ? $"Play {m_Selection.Count} card(s) as {m_PendingClaim}"
                : "Select at least one card";

            if (GUILayout.Button(caption, GUILayout.Height(30)))
            {
                decide.ConfirmDecision(m_PendingClaim, new List<CardID>(m_Selection));
            }

            GUI.enabled = true;
        }
    }
}
