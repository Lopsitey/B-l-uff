#region

using System;
using System.Collections.Generic;
using Template.Content.Scripts.Card.Data;
using Template.Content.Scripts.Card.Fuzzy;

#endregion

namespace Template.Content.Scripts.Card.Blackboard
{
    /// <summary>
    ///     Shared imperfect-information memory.
    ///     FSM states and fuzzy evaluators read and write here.
    /// </summary>
    internal sealed class GameBlackboard
    {
        public AIFuzzyProfile AIProfile { get; private set; }

        public GameBlackboard(AIFuzzyProfile profile)
        {
            ResetForNewRound(profile);
        }

        public TurnUser ActiveTurn { get; set; } = TurnUser.Player;

        /// <summary>Claimed target colour for the current pile play.</summary>
        public CardColour TargetColour { get; set; }

        /// <summary>Whether the last play was challenged by the defending seat.</summary>
        public bool LastPlayWasChallenged { get; set; }

        /// <summary>The actual cards committed in the most recent play.</summary>
        public List<CardID> LastPlayedCards { get; } = new List<CardID>(4);

        /// <summary>
        ///     Hidden trust from the current opponent toward the player (0 to 1).
        ///     Never shown to the player - decrement popups only.
        /// </summary>
        public float TrustTowardPlayer { get; private set; }

        /// <summary>How often the player has been caught bluffing this round (0 to 1-ish).</summary>
        public float PlayerBluffRateObserved { get; set; } //would have to be current bluff amount / total plays

        /// <summary>How often the opponent has been caught bluffing this round (0 to 1-ish).</summary>
        public float OpponentBluffRateObserved { get; set; }

        public List<CardID> PlayerHand { get; } = new List<CardID>(28);

        public List<CardID> OpponentHand { get; } = new List<CardID>(28);

        public List<CardID> Pile { get; } = new List<CardID>(28);

        public void ResetForNewRound(AIFuzzyProfile profile)
        {
            if (profile == null)
                throw new ArgumentNullException($"No opponent fuzzy profile selected: ", nameof(profile));

            AIProfile = profile;
            ActiveTurn = TurnUser.Player;
            LastPlayWasChallenged = false;
            TrustTowardPlayer = profile.StartingTrust;
            PlayerBluffRateObserved = 0.25f;
            OpponentBluffRateObserved = 0.25f;

            LastPlayedCards.Clear();
            PlayerHand.Clear();
            OpponentHand.Clear();
            Pile.Clear();
            InitialDraw();
        }

        public void SwapActiveTurn()
            => ActiveTurn = ActiveTurn == TurnUser.Player ? TurnUser.Opponent : TurnUser.Player;


        public void ShiftTrust(bool shiftUp)
        {
            // Increments the trust toward the player if the opponent was wrong, decrements if the opponent was right
            // As the player was honest so the opponent should trust them more, and vice versa
            if (shiftUp)
                TrustTowardPlayer += AIProfile.DistrustPerLie;
            else
                TrustTowardPlayer -= AIProfile.DistrustPerLie;
            TrustTowardPlayer = Math.Clamp(TrustTowardPlayer, 0f, 1f);
        }

        /// <summary>
        ///     Deals two 12-card hands and returns the 4 burned cards.
        ///     Leaves 4 cards out of play so neither player has perfect information
        /// </summary>
        private void InitialDraw()
        {
            var deck = Deck.Fill();
            Deck.Shuffle(deck);

            // Copies the first 12 cards to player1Hand, the next 12 to player2Hand
            PlayerHand.AddRange(new ArraySegment<CardID>(deck, 0, 12));
            OpponentHand.AddRange(new ArraySegment<CardID>(deck, 12, 12));

            // Wrap the remaining 4 cards directly into a Stack for drawing
            Pile.AddRange((new ArraySegment<CardID>(deck, 24, 4)));
        }

        /// <summary>
        ///     Gets the size of the active player's hand.
        /// </summary>
        public int GetActiveHandSize()
            => ActiveTurn == TurnUser.Player ? PlayerHand.Count : OpponentHand.Count;

        public string GetOpponentLabel() => AIProfile.DisplayName;
    }
}