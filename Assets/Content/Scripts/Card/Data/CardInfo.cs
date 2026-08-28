#region

using System;

#endregion

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
    public readonly struct CardID : IEquatable<CardID>
    {
        public CardID(CardSuit suit, CardColour colour)
        {
            Suit = suit;
            Colour = colour;
        }

        public CardSuit Suit { get; }
        public CardColour Colour { get; }

        // Explicit equality keeps hand lookups allocation-free; the default struct comparer boxes and uses reflection
        public bool Equals(CardID other) => Suit == other.Suit && Colour == other.Colour;
        public override bool Equals(object obj) => obj is CardID other && Equals(other);
        public override int GetHashCode() => ((int)Suit * ColourWheel.Count) + (int)Colour;
        public override string ToString() => $"{Colour} {Suit}";
    }

    /// <summary>
    ///     The colour wheel. Claims may only move one step around it, and the wheel wraps from Pink back to Red.
    /// </summary>
    public static class ColourWheel
    {
        public const int Count = 7;

        /// <summary>Moves around the wheel by <paramref name="steps" />, wrapping in both directions.</summary>
        public static CardColour Step(this CardColour colour, int steps)
        {
            var index = ((int)colour + steps) % Count;
            if (index < 0) index += Count;
            return (CardColour)index;
        }

        /// <summary>True when <paramref name="claim" /> is the same colour or one step either side of it.</summary>
        public static bool IsWithinOneStep(this CardColour colour, CardColour claim)
        {
            var gap = (int)colour - (int)claim;
            if (gap < 0) gap = -gap;
            return gap <= 1 || gap == Count - 1;
        }

        /// <summary>The three claims playable on top of <paramref name="colour" />: one back, the same, one forward.</summary>
        public static CardColour[] LegalClaims(CardColour colour)
            => new[] { colour.Step(-1), colour, colour.Step(1) };
    }
}