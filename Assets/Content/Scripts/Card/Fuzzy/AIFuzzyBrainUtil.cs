#region

using Template.Content.Scripts.Card.Blackboard;

#endregion

namespace Template.Content.Scripts.Card.Fuzzy
{
    internal static class AIFuzzyBrainUtil
    {
        /// <summary>
        ///     Urge to call the player's bluff (0 to 1).
        /// </summary>
        public static float EvaluateCallChance(GameBlackboard board)
        {
            // Distrust is good here because it increases the chance to call the more of the player's cards we know
            var distrust =
                1f - board
                    .TrustTowardPlayer; // Cancels out if the player has been honest as the trust would increase - 0.5 by default  
            var history = board.PlayerBluffRateObserved; // Times the player was caught bluffing this round (0 to 1-ish)
            var pilePressure = Clamp01(board.Pile.Count / 10f); // How many cards are in the pile (0 to 1)

            var intelligence = board.AIProfile.CallChance;
            var totalScore = (distrust * 0.4f) + (history * 0.3f) + (pilePressure * 0.2f) + (intelligence * 0.1f);
            return Clamp01(totalScore);
        }

        /// <summary>
        ///     Willingness to perform a bluff (0 to 1).
        /// </summary>
        public static float EvaluateBluffRisk(GameBlackboard board)
        {
            var handSize = board.GetActiveHandSize();
            var desperation = 1f - (handSize / 12f); // Goes riskier as the hand gets smaller
            desperation = Clamp01(desperation);

            // Used trust instead of distrust here because the more we trust the player the more we should bluff
            // This is because they are giving away perfect information as opposed to hoarding cards
            // Hoards are bad as if they are unplayed they are likely to match our unplayed cards
            var baseRisk = 0.35f + (desperation * 0.4f) + board.AIProfile.LieChance * 0.15f +
                           board.TrustTowardPlayer * 0.1f;

            return Clamp01(baseRisk);
        }

        /// <summary>
        ///     Willingness to drop multiple cards (0 to 1).
        ///     Honest play: scaled by availableComboStrength (what the hand actually supports).
        ///     Bluff: comboIntelligence only. Card availability is not required to attempt a multi-card lie.
        /// </summary>
        public static float EvaluateComboAppetite(GameBlackboard board, float availableComboStrength, bool isBluffing)
        {
            var baseChance = board.AIProfile.ComboChance;

            if (isBluffing)
                return Clamp01(baseChance);

            return availableComboStrength <= 0f ? 0f : Clamp01(baseChance * Clamp01(availableComboStrength));
        }

        private static float Clamp01(float value)
            => value < 0f ? 0f : value > 1f ? 1f : value;
    }
}