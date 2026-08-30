#region

using System.Collections.Generic;
using Template.Content.Scripts.Card.Blackboard;
using Template.Content.Scripts.Card.Data;
using Template.Content.Scripts.Card.Fsm;
using Template.Content.Scripts.Card.Fsm.States;
using Template.Content.Scripts.Card.Fuzzy;
using TMPro;
using UnityEngine;

#endregion

namespace Template.Content.Scripts.Managers
{
    /// <summary>
    ///     The stub that starts everything off. A small manager mainly used for its MonoBehaviour.
    ///     The real game management is in the FSM and blackboard. This manager is a singleton so that other systems can access
    ///     the blackboard and FSM.
    /// </summary>
    public sealed class GameManager : Singleton<GameManager>
    {
        [Tooltip("Required. Fuzzy profile for the god you face this round.")] [SerializeField]
        public AIFuzzyProfile m_AIProfile;

        /// <summary>Public accessors for important Blackboard vars.</summary>
        public List<CardID> PlayerHand => m_Blackboard.PlayerHand;

        public int OpponentHandSize => m_Blackboard.OpponentHand.Count;

        [Tooltip("The colour just put in the pot. May be null if no play has been made yet")]
        public CardColour TargetColour => m_Blackboard.TargetColour;

        public TurnUser ActiveTurn => m_Blackboard != null ? m_Blackboard.ActiveTurn : TurnUser.Player;

        public int SelectedCardsCount => m_SelectedCards.Count;

        public bool LastPlayWasChallenged => m_Blackboard.LastPlayWasChallenged;

        public ICardRoundState CurrentState => m_Fsm?.CurrentState;

        private GameBlackboard m_Blackboard;
        private FSM m_Fsm;

        public DialogueManager m_DialogueManager;

        public ArmManager m_PlayerArmManager;
        public ArmManager m_OpponentArmManager;

        [SerializeField] private List<DialogueLine> m_IntroDialogue;
        [SerializeField] private List<DialogueLine> m_PlayerAddedItemDialogue;
        [SerializeField] private List<DialogueLine> m_AIAddedItemDialogue;
        [SerializeField] private List<DialogueLine> m_CallOutWrongDialogue;
        [SerializeField] private List<DialogueLine> m_CallOutCorrectDialogue;
        [SerializeField] private List<DialogueLine> m_GetCalledOutWrongDialogue;
        [SerializeField] private List<DialogueLine> m_GetCalledOutCorrectDialogue;
        [SerializeField] private List<DialogueLine> m_WinDialogue;
        [SerializeField] private List<DialogueLine> m_LoseDialogue;

        [Header("Item Spawning")] [SerializeField]
        private GameObject m_ItemPrefab;

        [SerializeField] private Transform m_HandContainer;

        [Tooltip("Transform target where the selected item to play moves to in the player's hand/arm")] [SerializeField]
        private Transform m_HeldItemContainer;

        [SerializeField] private float m_ItemSpacing = 1.5f;

        [Tooltip("Maximum cards displayed per row before wrapping to a new row")] [SerializeField]
        private int m_CardsPerRow = 4;

        [Tooltip("Vertical offset applied between rows when hand wraps")] [SerializeField]
        private float m_RowYOffset = 1.2f;

        private readonly List<Item> m_SpawnedHandItems = new List<Item>();
        private readonly List<CardID> m_SelectedCards = new List<CardID>();
        private CardColour m_SelectedColour;

        /// <summary>
        ///     Instantiates visual items in the player's hand based on the current blackboard state.
        ///     Items wrap into rows according to m_CardsPerRow, with horizontal centering per row
        ///     and a vertical m_RowYOffset.
        /// </summary>
        public void SpawnPlayerHandItems()
        {
            ClearPlayerHandItems();

            if (m_ItemPrefab == null)
            {
                Debug.LogWarning("[GameManager] Item prefab is not assigned on GameManager.");
                return;
            }

            var hand = PlayerHand;
            if (hand == null || hand.Count == 0) return;

            var cardsPerRow = Mathf.Max(1, m_CardsPerRow);
            var parent = m_HandContainer != null ? m_HandContainer : transform;
            var basePos = m_HandContainer != null ? m_HandContainer.position : new Vector3(0f, -3f, 0f);

            for (var i = 0; i < hand.Count; i++)
            {
                var card = hand[i];
                var rowIndex = i / cardsPerRow;
                var colIndex = i % cardsPerRow;

                // Count items in the current row to centre each row independently
                var itemsInThisRow = Mathf.Min(cardsPerRow, hand.Count - (rowIndex * cardsPerRow));
                var rowStartX = -((itemsInThisRow - 1) * m_ItemSpacing) / 2f;

                var spawnPos = basePos + new Vector3(
                    rowStartX + (colIndex * m_ItemSpacing),
                    -rowIndex * m_RowYOffset,
                    0f
                );

                var itemObj = Instantiate(m_ItemPrefab, spawnPos, Quaternion.identity, parent);
                var itemComp = itemObj.GetComponent<Item>();
                if (itemComp != null)
                {
                    itemComp.Initialize(card.Colour, card.Suit);
                    // Tiered sorting to make the hand look nice
                    itemComp.SetSortingOrder(i * 2 + 1);
                    m_SpawnedHandItems.Add(itemComp);
                }
            }
        }

        // For drawing any bonus cards - debuffs etc.
        // If the player incorrectly calls the opponent the resolve state draws more cards with the function above. 
        public void DrawCards(int count)
        {
            if (m_Blackboard == null) return;
            m_Blackboard.DrawExtraCards(TurnUser.Player, count);
            SpawnPlayerHandItems();
        }

        private void ClearPlayerHandItems()
        {
            foreach (var t in m_SpawnedHandItems)
            {
                if (t != null)
                    Destroy(t.gameObject);
            }

            m_SpawnedHandItems.Clear();
        }

        /// <summary>
        ///     Removes the played cards from the player's hand items and destroys their game objects.
        /// </summary>
        /// <param name="playedCards"></param>
        private void RemovePlayedItems(List<CardID> playedCards)
        {
            for (var i = m_SpawnedHandItems.Count - 1; i >= 0; i--)
            {
                var item = m_SpawnedHandItems[i];
                if (item == null) continue;

                foreach (var t in playedCards)
                {
                    if (item.cardColour == t.Colour && item.cardSuit == t.Suit)
                    {
                        m_SpawnedHandItems.RemoveAt(i);
                        Destroy(item.gameObject);
                        break;
                    }
                }
            }
        }

        public void OnCardAdded(CardColour colour, CardSuit suit)
            => m_SelectedCards.Add(new CardID(suit, colour));

        public void OnCardRemoved(CardColour colour, CardSuit suit)
            => m_SelectedCards.Remove(new CardID(suit, colour));

        public void OnColourSelected(CardColour colour)
            => m_SelectedColour = colour;

        // Called when the player has finished playing the cards
        public void FinishPlay()
        {
            if (m_Fsm.CurrentState is PlayState playState && m_Blackboard.ActiveTurn == TurnUser.Player)
            {
                playState.CompletePlay();
                Debug.Log($"[GameManager] Player finished their play with {m_SelectedCards.Count} cards.");

                if (m_DialogueManager != null && m_PlayerAddedItemDialogue != null && m_PlayerAddedItemDialogue.Count > 0)
                    m_DialogueManager.SetNewDialogue(m_PlayerAddedItemDialogue);

                if (m_PlayerArmManager != null)
                    m_PlayerArmManager.RaiseArm();
            }
        }

        public void ConfirmPlayerDecision()
        {
            if (m_DialogueManager != null && m_DialogueManager.IsDialogueActive)
                return;

            if (m_SelectedCards.Count == 0)
            {
                if (m_PlayerArmManager != null)
                    m_PlayerArmManager.ErrorJiggle();
                return;
            }

            if (m_Fsm.CurrentState is DecideState decideState && m_Blackboard.ActiveTurn == TurnUser.Player)
            {
                var playedCardsCopy = new List<CardID>(m_SelectedCards);
                decideState.ConfirmDecision(m_SelectedColour, m_SelectedCards);
                RemovePlayedItems(playedCardsCopy);
                m_SelectedCards.Clear();
                FinishPlay();
            }
        }

        public void ChallengeOpponent()
        {
            if (m_Fsm.CurrentState is ReactState reactState && m_Blackboard.ActiveTurn == TurnUser.Opponent)
            {
                if (m_OpponentArmManager != null)
                    m_OpponentArmManager.RevealItem();
                reactState.Challenge();
            }
            // Can also call player challenge dialogue here
        }

        public void Pass()
        {
            if (m_Fsm.CurrentState is ReactState reactState && m_Blackboard.ActiveTurn == TurnUser.Opponent)
            {
                if (m_OpponentArmManager != null)
                    m_OpponentArmManager.DropItem();
                reactState.Pass();
            }
        }


        public void ChallengeOpponentCorrect()
        {
            if (m_DialogueManager != null)
                m_DialogueManager.SetNewDialogue(m_CallOutCorrectDialogue);
        }

        public void ChallengeOpponentWrong()
        {
            if (m_DialogueManager != null)
                m_DialogueManager.SetNewDialogue(m_CallOutWrongDialogue);
        }


        public void AIDialogueChallenge()
        {
            //not needed?
        }

        public void AIDialogueChallengeCorrect()
        {
            if (m_DialogueManager != null)
                m_DialogueManager.SetNewDialogue(m_GetCalledOutCorrectDialogue);
        }

        public void AIDialogueChallengeWrong()
        {
            if (m_DialogueManager != null)
                m_DialogueManager.SetNewDialogue(m_GetCalledOutWrongDialogue);
        }

        public void AIAddCards()
        {
            if (m_OpponentArmManager != null)
                m_OpponentArmManager.RaiseArm();
            if (m_DialogueManager != null)
                m_DialogueManager.SetNewDialogue(m_AIAddedItemDialogue);
        }

        [Header("Win / Loss UI")] [SerializeField]
        private TMP_Text m_WinLossText;

        public void EndDialogue(bool playerWon)
        {
            // Called when the player has ended the round
            if (m_WinLossText != null)
            {
                m_WinLossText.gameObject.SetActive(true);
                m_WinLossText.text = playerWon ? "VICTORY!" : "DEFEATED...";
                m_WinLossText.color = playerWon ? Color.green : Color.red;
            }

            if (playerWon) //Use for win/loss dialogue
            {
                if (m_DialogueManager != null)
                    m_DialogueManager.SetNewDialogue(m_WinDialogue);
                Debug.Log($"[GameManager] Player won the round vs {m_AIProfile.DisplayName}.");
            }
            else
            {
                if (m_DialogueManager != null)
                    m_DialogueManager.SetNewDialogue(m_LoseDialogue);
                Debug.Log($"[GameManager] Player lost the round vs {m_AIProfile.DisplayName}.");
            }
        }

        private void Start()
        {
            if (!HasOpponentProfile()) return;

            BeginRound();
        }

        private void BeginRound()
        {
            // Instantiates the round blackboard and FSM
            m_Blackboard = new GameBlackboard(m_AIProfile);
            m_Fsm = new FSM();
            m_Fsm.SetState(new DecideState(m_Fsm, m_Blackboard));

            Debug.Log($"[GameManager] Round started vs {m_AIProfile.DisplayName}.");

            SpawnPlayerHandItems();

            if (m_DialogueManager != null)
                m_DialogueManager.SetNewDialogue(m_IntroDialogue);
        }

        public void ResetRound()
        {
            if (m_AIProfile != null)
                BeginRound();
        }

        private bool HasOpponentProfile()
        {
            if (m_AIProfile != null)
                return true;

            Debug.LogError("[GameManager] AIFuzzyProfile is required. Assign one on GameManager.",
                this);
            enabled = false;
            return false;
        }
    }
}