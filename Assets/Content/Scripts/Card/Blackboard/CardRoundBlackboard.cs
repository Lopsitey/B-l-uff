using System.Collections.Generic;
using Bluff.Card.Data;

namespace Content.Scripts.Card.Blackboard
{
    /// <summary>
    /// Shared imperfect-information memory for one card round.
    /// FSM states and fuzzy evaluators read and write here. No combat link yet.
    /// </summary>
    public sealed class CardRoundBlackboard
    {
        /// <summary>Default opponents for the first (and currently only) card table.</summary>
        public const int DefaultEnemyCount = 2;

        /// <summary>Soft cap so arrays stay sized. Raise later if multi-floor returns.</summary>
        public int MaxEnemies { get; set; } = 3;

        public int EnemyCount { get; set; } = DefaultEnemyCount;

        public int CurrentPlayerIndex { get; set; }

        /// <summary>Claimed target rank for the current pile play (Cheat-style).</summary>
        public CardRank TargetRank { get; set; }

        public int PileSize { get; set; }

        /// <summary>
        /// Per-opponent hidden trust. 1 = trusts you, 0 = does not.
        /// Never show this to the player. Flavour popup can say "X will remember that"
        /// while this float is what actually changes.
        /// </summary>
        public float[] TrustTowardPlayer { get; private set; }

        /// <summary>How often each seat has been caught bluffing (0 to 1-ish).</summary>
        public float[] ObservedBluffRate { get; private set; }

        public List<DebuffCardId> PlayerHand { get; } = new List<DebuffCardId>();

        /// <summary>One list per enemy seat. Keep length in sync with EnemyCount.</summary>
        public List<DebuffCardId>[] EnemyHands { get; private set; }

        public List<DebuffCardId> DiscardHistory { get; } = new List<DebuffCardId>();

        /// <summary>
        /// Outcome scratch for the round. Combat will read this later.
        /// Leave unset for now. Do not call into combat from here.
        /// </summary>
        public float PendingCombatBuffStrength { get; set; }

        public float PendingCombatDebuffStrength { get; set; }

        public void ResetForNewRound(int enemyCount)
        {
            if (enemyCount < 1)
            {
                enemyCount = DefaultEnemyCount;
            }

            if (enemyCount > MaxEnemies)
            {
                enemyCount = MaxEnemies;
            }

            EnemyCount = enemyCount;
            CurrentPlayerIndex = 0;
            PileSize = 0;
            PendingCombatBuffStrength = 0f;
            PendingCombatDebuffStrength = 0f;

            PlayerHand.Clear();
            DiscardHistory.Clear();

            TrustTowardPlayer = new float[EnemyCount];
            ObservedBluffRate = new float[EnemyCount];
            EnemyHands = new List<DebuffCardId>[EnemyCount];

            for (var i = 0; i < EnemyCount; i++)
            {
                TrustTowardPlayer[i] = 0.5f;
                ObservedBluffRate[i] = 0.25f;
                EnemyHands[i] = new List<DebuffCardId>();
            }
        }

        public void ApplyCaughtLieTrustHit(int enemyIndex, float amount = 0.1f)
        {
            if (TrustTowardPlayer == null || enemyIndex < 0 || enemyIndex >= TrustTowardPlayer.Length)
            {
                return;
            }

            TrustTowardPlayer[enemyIndex] -= amount;
            if (TrustTowardPlayer[enemyIndex] < 0f)
            {
                TrustTowardPlayer[enemyIndex] = 0f;
            }
        }
    }
}
