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
        ///     Checks if the card's colour is within the threshold of the target colour.
        /// </summary>
        /// <param name="target">The target colour to check against.</param>
        /// <returns>True if the card's colour is within the threshold, false otherwise.</returns>
        public bool IsColourOutsideThreshold(CardColour target)
        {
            // +6 is the same as -1 due to the wrap around the modulo allows
            // Can't be negative as the modulo would return negative
            var lower = (CardColour)(((int)Colour + 6) % 7);
            var higher = (CardColour)(((int)Colour + 1) % 7);
            return Colour != target && lower != target && higher != target;
        }

        public CardSuit Suit { get; }

        public CardColour Colour { get; }
    }
}