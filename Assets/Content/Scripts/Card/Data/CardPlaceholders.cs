namespace Bluff.Card.Data
{
    /// <summary>
    /// Placeholder suit and rank ids until real debuff names land.
    /// Rename these when design locks. Do not treat as final flavour.
    /// </summary>
    public enum CardSuit
    {
        SuitA = 0,
        SuitB = 1,
        SuitC = 2,
        SuitD = 3
    }

    public enum CardRank
    {
        Rank1 = 1,
        Rank2 = 2,
        Rank3 = 3,
        Rank4 = 4,
        Rank5 = 5
    }

    /// <summary>
    /// Minimal card identity. Expand later (debuff effect, art id, etc.).
    /// </summary>
    public readonly struct DebuffCardId
    {
        public DebuffCardId(CardSuit suit, CardRank rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public CardSuit Suit { get; }
        public CardRank Rank { get; }
    }
}
