#region

using UnityEngine;

#endregion

namespace Template.Content.Scripts.Card.Data
{
    public static class Deck
    {
        /// <summary>
        ///     Creates a new deck of cards.
        /// </summary>
        /// <returns>A new array of CardID objects.</returns>
        public static CardID[] Fill()
        {
            var deck = new CardID[28];
            int index = 0;
            for (int suit = 0; suit < 4; ++suit)
            {
                for (int colour = 0; colour < 7; ++colour)
                {
                    // Iterates through each suit, then every colour, using the external index to fill the entire deck.
                    deck[index++] = new CardID((CardSuit)suit, (CardColour)colour);
                }
            }

            return deck;
        }

        /// <summary>
        ///     In-place Fisher-Yates shuffle - like a bubble sort but with random swaps.
        /// </summary>
        /// <param name="deck">The deck to shuffle.</param>
        public static void Shuffle(CardID[] deck)
        {
            for (int i = deck.Length - 1; i > 0; i--)
            {
                // Generate a random index between 0 and i (inclusive)
                int randomIndex = Random.Range(0, i + 1);

                // Swap
                CardID temp = deck[i];
                deck[i] = deck[randomIndex];
                deck[randomIndex] = temp;
            }
        }
    }
}