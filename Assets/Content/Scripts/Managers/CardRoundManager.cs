using Bluff.Card.Fsm;
using Bluff.Card.Fsm.States;
using Bluff.Card.Fuzzy;
using Content.Scripts.Card.Blackboard;
using Template.Core;
using UnityEngine;

namespace Bluff.Managers
{
    /// <summary>
    /// Owns one Cheat-style card table session.
    /// </summary>
    public sealed class CardRoundManager : Singleton<CardRoundManager>
    {
        [SerializeField]
        [Tooltip("Opponents at the table for this round.")]
        private int m_EnemyCount = CardRoundBlackboard.DefaultEnemyCount;

        [SerializeField]
        [Tooltip("Soft max seats. Blackboard clamps to this.")]
        private int m_MaxEnemies = 3;

        private CardRoundBlackboard m_Blackboard;
        private BluffFuzzyEvaluator m_Fuzzy;
        private CardRoundFsm m_Fsm;

        public CardRoundBlackboard Blackboard => m_Blackboard;

        protected override void Awake()
        {
            base.Awake();
            m_Blackboard = new CardRoundBlackboard
            {
                MaxEnemies = m_MaxEnemies
            };
            m_Fuzzy = new BluffFuzzyEvaluator();
            m_Fsm = new CardRoundFsm(m_Blackboard, m_Fuzzy);
        }

        private void Start()
        {
            BeginRound(m_EnemyCount);
        }

        private void Update()
        {
            if (m_Fsm != null)
            {
                m_Fsm.Tick();
            }
        }

        public void BeginRound(int enemyCount)
        {
            m_Blackboard.MaxEnemies = m_MaxEnemies;
            m_Blackboard.ResetForNewRound(enemyCount);
            m_Fsm.SetState(new DrawState(m_Fsm, m_Blackboard));
            Debug.Log($"[CardRoundManager] Round started with {m_Blackboard.EnemyCount} enemies.");
        }
    }
}
