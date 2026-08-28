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

        public CardSuit Suit { get; }
        public CardColour Colour { get; }
    }
}