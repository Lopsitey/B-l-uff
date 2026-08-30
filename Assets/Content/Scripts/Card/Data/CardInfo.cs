namespace Template.Content.Scripts.Card.Data
{
    public enum CardSuit
    {
        Gems = 0,
        Flora = 1,
        Flesh = 2,
        Vials = 3
    }

    public enum CardColour
    {
        Red = 0,
        Orange = 1,
        Yellow = 2,
        Green = 3,
        Blue = 4,
        Purple = 5,
        Pink = 6
    }

    /// <summary>
    ///     Suit and colour - for a single card.
    ///     This is the only data needed to represent a card in memory.
    /// </summary>
    public readonly struct CardID
    {
        public CardID(CardSuit suit, CardColour colour)
        {
            Suit = suit;
            Colour = colour;
        }
        
        /// <summary>
        ///     Checks if the card's colour is within 1 step (+-1 with wrap) of the target colour.
        /// </summary>
        public bool IsColourWithinThreshold(CardColour target)
        {
            CardHelpers.GetNeighbouringColours(out var lower, out var higher, target);
            return Colour == target || lower == target || higher == target;
        }

        /// <summary>
        ///     Checks if the card's colour is outside the threshold of the target colour.
        /// </summary>
        /// <param name="target">The target colour to check against.</param>
        /// <returns>True if the card's colour is outside the threshold, false otherwise.</returns>
        public bool IsColourOutsideThreshold(CardColour target)
            => !IsColourWithinThreshold(target);

        public CardSuit Suit { get; }

        public CardColour Colour { get; }
    }

    public static class CardHelpers
    {
        /// <summary>
        ///     Gets the neighbouring colours of the card's colour, considering wrap-around.
        /// </summary>
        /// <param name="lower">The lower neighbouring colour.</param>
        /// <param name="higher">The higher neighbouring colour.</param>
        /// <param name="targetColour">The target colour to get neighbouring colours for.</param>
        public static void GetNeighbouringColours(out CardColour lower, out CardColour higher, CardColour targetColour)
        {
            lower = (CardColour)(((int)targetColour + 6) % 7);
            higher = (CardColour)(((int)targetColour + 1) % 7);
        }
    }
}