#region

using Template.Content.Scripts.Card.Blackboard;
using Template.Content.Scripts.Card.Fsm;
using Template.Content.Scripts.Card.Fsm.States;
using Template.Content.Scripts.Card.Fuzzy;
using UnityEngine;

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
        [Tooltip("Required. Fuzzy profile for the god you face this round.")] [SerializeField]
        private AIFuzzyProfile m_AIProfile;

        private GameBlackboard m_Blackboard;
        private AIFuzzyBrain m_Fuzzy;
        private FSM m_Fsm;

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

            Debug.LogError("[GameManager] AIFuzzyProfile is required. Assign one on GameManager.",
                this);
            enabled = false;
            return false;
        }
    }
}