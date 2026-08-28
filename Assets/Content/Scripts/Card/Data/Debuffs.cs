#region

using System;

#endregion

namespace Template.Content.Scripts.Card.Data
{
    /// <summary>
    ///     Handler for applying debuffs triggered by card colours.
    /// </summary>
    public static class Debuffs
    {
        public static void ApplyCardDebuff(CardColour colour)
        {
            switch (colour)
            {
                case CardColour.Red:
                    Love();
                    break;
                case CardColour.Orange:
                    Midas();
                    break;
                case CardColour.Yellow:
                    Stone();
                    break;
                case CardColour.Green:
                    Blind();
                    break;
                case CardColour.Blue:
                    Shaky();
                    break;
                case CardColour.Purple:
                    Invisibility();
                    break;
                case CardColour.Pink:
                    Truth();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(colour), colour, $"Unknown debuff colour: {colour}");
            }
        }

        public static void ApplyCardDebuff(CardID card)
        {
            ApplyCardDebuff(card.Colour);
        }

        private static void Love()
        {
            // TODO: Implement Love debuff effect
        }

        private static void Midas()
        {
            // TODO: Implement Midas debuff effect
        }

        private static void Stone()
        {
            // TODO: Implement Stone debuff effect
        }

        private static void Blind()
        {
            // TODO: Implement Blind debuff effect
        }

        private static void Shaky()
        {
            // TODO: Implement Shaky debuff effect
        }

        private static void Invisibility()
        {
            // TODO: Implement Invisibility debuff effect
        }

        private static void Truth()
        {
            // TODO: Implement Truth debuff effect
        }
    }
}
