#region

using System;
using System.Collections.Generic;
using Template.Content.Scripts.Card.Data;
using Template.Content.Scripts.Card.Fuzzy;
using UnityEngine;

#endregion

namespace Template.Content.Scripts.Card.Blackboard
{
    /// <summary>
    ///     Shared imperfect-information memory.
    ///     FSM states and fuzzy evaluators read and write here.
    /// </summary>
    internal sealed class GameBlackboard
    {
        private const int StartingHandSize = 12;

        private int m_PlayerReveals;
        private int m_PlayerCaughtLies;
        private int m_OpponentReveals;
        private int m_OpponentCaughtLies;

        public GameBlackboard(AIFuzzyProfile profile)
        {
            ResetForNewRound(profile);
        }

        public AIFuzzyProfile AIProfile { get; private set; }

        public TurnUser ActiveTurn { get; set; } = TurnUser.Player;

        /// <summary>The seat that reacts to the active seat's play.</summary>
        public TurnUser DefendingSeat => ActiveTurn == TurnUser.Player ? TurnUser.Opponent : TurnUser.Player;

        /// <summary>Claimed colour for the current pile. Claims may only move one step around the wheel.</summary>
        public CardColour TargetColour { get; set; }

        /// <summary>Cards face-down in the pot. Whoever loses a challenge picks all of them up.</summary>
        public List<CardID> Pile { get; } = new List<CardID>(28);

        public int PileSize => Pile.Count;

        /// <summary>True when the next play may open with any colour because the pot is empty.</summary>
        public bool PileIsOpen => Pile.Count == 0;

        /// <summary>Whether the last play was challenged by the defending seat.</summary>
        public bool LastPlayWasChallenged { get; set; }

        /// <summary>The actual cards committed in the most recent play.</summary>
        public List<CardID> LastPlayedCards { get; } = new List<CardID>(4);

        /// <summary>Set once a seat empties their hand, which ends the round.</summary>
        public TurnUser? RoundWinner { get; set; }

        /// <summary>
        ///     Hidden trust from the current opponent toward the player (0 to 1).
        ///     Never shown to the player - decrement popups only.
        /// </summary>
        public float TrustTowardPlayer { get; private set; }

        /// <summary>How often the player has been caught bluffing this round (0 to 1-ish).</summary>
        public float PlayerBluffRateObserved { get; private set; }

        /// <summary>How often the opponent has been caught bluffing this round (0 to 1-ish).</summary>
        public float OpponentBluffRateObserved { get; private set; }

        public List<CardID> PlayerHand { get; } = new List<CardID>(28);

        public List<CardID> OpponentHand { get; } = new List<CardID>(28);

        /// <summary>The cards held out of the initial deal, drawn from as a penalty stack.</summary>
        public Stack<CardID> DrawStack { get; private set; }

        public void ResetForNewRound(AIFuzzyProfile profile)
        {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile), "No opponent fuzzy profile selected.");

            AIProfile = profile;
            ActiveTurn = TurnUser.Player;
            TargetColour = CardColour.Red;
            LastPlayWasChallenged = false;
            RoundWinner = null;

            TrustTowardPlayer = profile.StartingTrust;
            PlayerBluffRateObserved = 0.25f;
            OpponentBluffRateObserved = 0.25f;
            m_PlayerReveals = 0;
            m_PlayerCaughtLies = 0;
            m_OpponentReveals = 0;
            m_OpponentCaughtLies = 0;

            Pile.Clear();
            LastPlayedCards.Clear();
            PlayerHand.Clear();
            OpponentHand.Clear();
            DrawStack?.Clear();
            InitialDraw();
        }

        public List<CardID> GetHand(TurnUser user) => user == TurnUser.Player ? PlayerHand : OpponentHand;

        public void SwapActiveTurn() => ActiveTurn = DefendingSeat;

        /// <summary>
        ///     A claim is legal if the pot is empty (free choice) or it sits within one step of the current claim.
        /// </summary>
        public bool IsLegalClaim(CardColour claim) => PileIsOpen || TargetColour.IsWithinOneStep(claim);

        /// <summary>The claims playable right now: every colour on an open pot, otherwise the current one +-1.</summary>
        public CardColour[] GetLegalClaims()
        {
            if (!PileIsOpen) return ColourWheel.LegalClaims(TargetColour);

            var all = new CardColour[ColourWheel.Count];
            for (int i = 0; i < ColourWheel.Count; i++) all[i] = (CardColour)i;
            return all;
        }

        /// <summary>Moves the whole pot into a hand, which is the penalty for losing a challenge.</summary>
        public void TakePile(TurnUser user)
        {
            GetHand(user).AddRange(Pile);
            Pile.Clear();
            LastPlayedCards.Clear();
        }

        /// <summary>
        ///     Records what a revealed play turned out to be, which is the only honest way either side learns
        ///     how often the other lies.
        /// </summary>
        public void RecordReveal(TurnUser seat, bool wasBluff)
        {
            if (seat == TurnUser.Player)
            {
                m_PlayerReveals++;
                if (wasBluff) m_PlayerCaughtLies++;
                PlayerBluffRateObserved = m_PlayerCaughtLies / (float)m_PlayerReveals;

                TrustTowardPlayer += wasBluff ? -AIProfile.DistrustPerLie : AIProfile.DistrustPerLie * 0.5f;
                TrustTowardPlayer = Mathf.Clamp01(TrustTowardPlayer);
                return;
            }

            m_OpponentReveals++;
            if (wasBluff) m_OpponentCaughtLies++;
            OpponentBluffRateObserved = m_OpponentCaughtLies / (float)m_OpponentReveals;
        }

        /// <summary>
        ///     Draws up to <paramref name="count" /> cards from the penalty stack into the given hand.
        /// </summary>
        public void DrawExtraCards(TurnUser user, int count = 3)
        {
            var hand = GetHand(user);
            for (int i = 0; i < count && DrawStack.Count > 0; ++i)
                hand.Add(DrawStack.Pop());
        }

        /// <summary>
        ///     Deals two 12-card hands and holds back the remaining 4 cards.
        ///     Leaves 4 cards out of play so neither side has perfect information.
        /// </summary>
        private void InitialDraw()
        {
            var deck = Deck.Fill();
            Deck.Shuffle(deck);

            PlayerHand.AddRange(new ArraySegment<CardID>(deck, 0, StartingHandSize));
            OpponentHand.AddRange(new ArraySegment<CardID>(deck, StartingHandSize, StartingHandSize));

            // Wrap the leftover cards directly into a stack for penalty draws
            var dealt = StartingHandSize * 2;
            DrawStack = new Stack<CardID>(new ArraySegment<CardID>(deck, dealt, deck.Length - dealt));
        }

        /// <summary>
        ///     Gets the size of the active player's hand.
        /// </summary>
        public int GetActiveHandSize() => GetHand(ActiveTurn).Count;

        /// <summary>Counts cards of a given colour in a hand, used for honest-play decisions.</summary>
        public int CountColourInHand(TurnUser user, CardColour colour)
        {
            var hand = GetHand(user);
            var total = 0;
            for (int i = 0; i < hand.Count; i++)
                if (hand[i].Colour == colour)
                    total++;

            return total;
        }

        public string GetOpponentLabel() => AIProfile.DisplayName;
    }
}
