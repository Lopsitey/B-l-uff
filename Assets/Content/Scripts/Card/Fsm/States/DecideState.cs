#region

using System.Collections;
using System.Collections.Generic;
using Template.Content.Scripts.Card.Blackboard;
using Template.Content.Scripts.Card.Data;
using Template.Content.Scripts.Managers;
using UnityEngine;
using static Template.Content.Scripts.Card.Fuzzy.AIFuzzyBrainUtil;

#endregion

namespace Template.Content.Scripts.Card.Fsm.States
{
    internal sealed class DecideState : ICardRoundState
    {
        private readonly FSM m_Fsm;
        private readonly GameBlackboard m_Board;
        private Coroutine m_DecideCoroutine;

        public DecideState(FSM fsm, GameBlackboard board)
        {
            m_Fsm = fsm;
            m_Board = board;
        }

        public void Enter()
        {
            if (m_Board.ActiveTurn == TurnUser.Player)
            {
                Debug.Log($"[CardRound] Decide.Enter seat={m_Board.ActiveTurn} player vs {m_Board.GetOpponentLabel()}");
                GameManager.Instance.m_DialogueManager.SetNewDialogue(GameManager.Instance.m_DecidingDialogue);
            }
            else
            {
                if (GameManager.Instance != null)
                    m_DecideCoroutine = GameManager.Instance.StartCoroutine(OpponentDecideRoutine());
            }
        }

        private IEnumerator OpponentDecideRoutine()
        {
            yield return new WaitForSeconds(0.4f);

            var dialogueMgr = GameManager.Instance != null ? GameManager.Instance.m_DialogueManager : null;
            while (dialogueMgr != null && dialogueMgr.IsDialogueActive)
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.4f);

            ExecuteAIDecision();
        }

        private void ExecuteAIDecision()
        {
            var hand = m_Board.OpponentHand;
            Debug.Log(
                $"[CardRound] Decide.Enter seat={m_Board.ActiveTurn} opponent vs {m_Board.GetOpponentLabel()} (Hand count: {hand.Count})");

            if (hand.Count == 0)
                return;

            const float fuzzyThreshold = 0.85f;
            const float comboThreshold = 0.5f;

            var bluffRisk = EvaluateBluffRisk(m_Board);
            var isBluffing = bluffRisk > fuzzyThreshold;
            Debug.Log($"[CardRound] AI Bluff Evaluation: risk={bluffRisk:0.00}, threshold={fuzzyThreshold:0.00}, decidesBluff={isBluffing}");

            // Gather cards that match legal claims (target, target - 1, target + 1)
            var currentTarget = m_Board.TargetColour;
            CardHelpers.GetNeighbouringColours(out var legalMinus1, out var legalPlus1, currentTarget);

            var exactMatches = new List<CardID>();
            var minus1Matches = new List<CardID>();
            var plus1Matches = new List<CardID>();
            var nonMatchingCards = new List<CardID>();

            foreach (var card in hand)
            {
                if (card.Colour == currentTarget)
                    exactMatches.Add(card);
                else if (card.Colour == legalMinus1)
                    minus1Matches.Add(card);
                else if (card.Colour == legalPlus1)
                    plus1Matches.Add(card);
                else
                    nonMatchingCards.Add(card);
            }

            CardColour chosenClaim;
            var cardsToPlay = new List<CardID>();

            // Determine claim: prefer honest play if we have matching cards and aren't forcing a bluff
            if (!isBluffing && (exactMatches.Count > 0 || minus1Matches.Count > 0 || plus1Matches.Count > 0))
            {
                // Pick the legal claim where AI holds the most cards
                if (exactMatches.Count >= minus1Matches.Count && exactMatches.Count >= plus1Matches.Count)
                {
                    chosenClaim = currentTarget;
                    cardsToPlay.AddRange(exactMatches);
                }
                else if (minus1Matches.Count >= plus1Matches.Count)
                {
                    chosenClaim = legalMinus1;
                    cardsToPlay.AddRange(minus1Matches);
                }
                else
                {
                    chosenClaim = legalPlus1;
                    cardsToPlay.AddRange(plus1Matches);
                }

                var availableCombo = cardsToPlay.Count / 3f;
                var honestComboAppetite = EvaluateComboAppetite(m_Board, availableCombo, false);
                Debug.Log($"[CardRound] AI Honest Play: claim={chosenClaim}, matchingCardsAvailable={cardsToPlay.Count}, comboAppetite={honestComboAppetite:0.00}");

                var maxCards = honestComboAppetite > comboThreshold ? Mathf.Min(cardsToPlay.Count, 3) : 1;
                cardsToPlay = cardsToPlay.GetRange(0, maxCards);
            }
            else
            {
                // Forced lie or strategic bluff: Pick target or adjacent claim and dump junk/non-matching cards
                chosenClaim = isBluffing ? legalPlus1 : currentTarget;
                var bluffComboAppetite = EvaluateComboAppetite(m_Board, 0f, true);
                var wantedCount = bluffComboAppetite > comboThreshold ? 2 : 1;
                wantedCount = Mathf.Min(wantedCount, hand.Count);

                Debug.Log($"[CardRound] AI Bluff Play: claim={chosenClaim}, wantedCount={wantedCount}, bluffComboAppetite={bluffComboAppetite:0.00}");

                // Play junk cards first if available, otherwise any cards in hand
                if (nonMatchingCards.Count >= wantedCount)
                {
                    cardsToPlay = nonMatchingCards.GetRange(0, wantedCount);
                }
                else
                {
                    cardsToPlay = hand.GetRange(0, wantedCount);
                }
            }

            Debug.Log($"[CardRound] AI confirms play: {cardsToPlay.Count} card(s) claimed as {chosenClaim} (Target was: {currentTarget})");
            ConfirmDecision(chosenClaim, cardsToPlay);
        }

        // Called when decision is ready (by AI in Enter or by Player UI button callback)
        // Once cards + claim are decided, transition to PlayState
        public void ConfirmDecision(CardColour claimedColour, List<CardID> chosenCards)
        {
            m_Board.TargetColour = claimedColour;

            // Clean transition into PlayState
            m_Fsm.SetState(new PlayState(m_Fsm, m_Board, chosenCards));

            Debug.Log("CONFIRMED DECISION. ITEMS IN CAULDRON");
        }

        public void Exit()
        {
            if (m_DecideCoroutine != null && GameManager.Instance != null)
            {
                GameManager.Instance.StopCoroutine(m_DecideCoroutine);
                m_DecideCoroutine = null;
            }
        }
    }
}