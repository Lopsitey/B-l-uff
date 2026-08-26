using Content.Scripts.Card.Blackboard;

namespace Bluff.Card.Fuzzy
{
    /// <summary>
    /// Continuous suspicion / risk helpers. Not boolean AI.
    /// Membership curves and rule weights stay empty until we tune against playtests.
    /// </summary>
    public sealed class BluffFuzzyEvaluator
    {
        /// <summary>
        /// Inputs (planned): pile size, claimed card count, opponent observed bluff rate,
        /// hidden trust toward player, cards left in hand.
        /// Output: 0 to 1 challenge urge.
        /// </summary>
        public float EvaluateChallengeChance(CardRoundBlackboard board, int observerIndex)
        {
            if (board == null || board.TrustTowardPlayer == null)
            {
                return 0.5f;
            }

            if (observerIndex < 0 || observerIndex >= board.TrustTowardPlayer.Length)
            {
                return 0.5f;
            }

            // Stub mix so callers have something to wire. Replace with real fuzzy curves.
            var distrust = 1f - board.TrustTowardPlayer[observerIndex];
            var history = board.ObservedBluffRate[observerIndex];
            var pilePressure = board.PileSize / 10f;
            if (pilePressure > 1f)
            {
                pilePressure = 1f;
            }

            var score = (distrust * 0.45f) + (history * 0.35f) + (pilePressure * 0.2f);
            if (score < 0f)
            {
                return 0f;
            }

            if (score > 1f)
            {
                return 1f;
            }

            return score;
        }

        /// <summary>
        /// Inputs (planned): cards remaining, distance from emptying hand, current trust,
        /// how wild the lie is (card count).
        /// Output: 0 to 1 willingness to bluff.
        /// </summary>
        public float EvaluateBluffRisk(CardRoundBlackboard board, int actorIndex)
        {
            if (board == null)
            {
                return 0.5f;
            }

            var handSize = board.PlayerHand != null ? board.PlayerHand.Count : 0;
            if (actorIndex > 0 && board.EnemyHands != null && actorIndex - 1 < board.EnemyHands.Length)
            {
                var enemyHand = board.EnemyHands[actorIndex - 1];
                handSize = enemyHand != null ? enemyHand.Count : 0;
            }

            // Fewer cards left => more desperate => slightly higher bluff appetite.
            var desperation = 1f - (handSize / 8f);
            if (desperation < 0f)
            {
                desperation = 0f;
            }

            if (desperation > 1f)
            {
                desperation = 1f;
            }

            return 0.35f + (desperation * 0.4f);
        }
    }
}
