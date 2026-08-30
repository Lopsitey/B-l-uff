#region

using System.Collections;
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
        /// <summary>Public accessors for important Blackboard vars.</summary>
        public List<CardID> PlayerHand => m_Blackboard.PlayerHand;

        public int OpponentHandSize => m_Blackboard.OpponentHand.Count;

        [Tooltip("The colour just put in the pot. May be null if no play has been made yet")]
        public CardColour TargetColour => m_Blackboard.TargetColour;

        public TurnUser ActiveTurn => m_Blackboard != null ? m_Blackboard.ActiveTurn : TurnUser.Player;

        public int SelectedCardsCount => m_SelectedCards.Count;

        public CardColour SelectedColour => m_SelectedColour;

        public bool LastPlayWasChallenged => m_Blackboard.LastPlayWasChallenged;

        public bool IsActionInProgress => m_IsActionInProgress;

        public ICardRoundState CurrentState => m_Fsm?.CurrentState;

        private GameBlackboard m_Blackboard;
        private FSM m_Fsm;
        private bool m_IsActionInProgress;

        [Header("Managers")] [SerializeField] public DialogueManager m_DialogueManager;
        public ArmManager m_PlayerArmManager;
        public ArmManager m_OpponentArmManager;

        [Header("AI Profile")] [Tooltip("Required. Fuzzy profile for the god you face this round.")] [SerializeField]
        public AIFuzzyProfile m_AIProfile;

        [Header("Dialogue")] [SerializeField] private List<DialogueLine> m_IntroDialogue;
        [SerializeField] private List<DialogueLine> m_PlayerAddedItemDialogue;
        [SerializeField] private List<DialogueLine> m_AIAddedItemDialogue;
        [SerializeField] private List<DialogueLine> m_CallOutWrongDialogue;
        [SerializeField] private List<DialogueLine> m_CallOutCorrectDialogue;
        [SerializeField] private List<DialogueLine> m_GetCalledOutWrongDialogue;
        [SerializeField] private List<DialogueLine> m_GetCalledOutCorrectDialogue;
        [SerializeField] private List<DialogueLine> m_WinDialogue;
        [SerializeField] private List<DialogueLine> m_LoseDialogue;
        [SerializeField] public List<DialogueLine> m_DecidingDialogue;

        [Header("Win / Loss UI")] [SerializeField]
        private TMP_Text m_WinLossText;

        [Header("Item Spawning")] [SerializeField]
        private GameObject m_ItemPrefab;

        [SerializeField] private SpriteRenderer waterSpriteRenderer;

        [SerializeField] private Transform m_HandContainer;

        [Tooltip("Transform target where the selected item to play moves to in the player's hand/arm")] [SerializeField]
        private Transform m_HeldItemContainer;

        [SerializeField] private float m_ItemSpacing = 1.5f;

        [Tooltip("Maximum cards displayed per row before wrapping to a new row")] [SerializeField]
        private int m_CardsPerRow = 4;

        [Tooltip("Vertical offset applied between rows when hand wraps")] [SerializeField]
        private float m_RowYOffset = 1.2f;

        private readonly List<Item> m_SpawnedHandItems = new List<Item>();
        private readonly List<Item> m_SelectedItems = new List<Item>();
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
                    var order = (i + 1) * 2 + 1;
                    itemComp.SetHandOrigin(parent, itemObj.transform.localPosition, order);
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
            m_SelectedItems.Clear();
            m_SelectedCards.Clear();

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
            foreach (var item in m_SelectedItems)
            {
                if (item != null)
                {
                    m_SpawnedHandItems.Remove(item);
                    Destroy(item.gameObject);
                }
            }

            m_SelectedItems.Clear();
            m_SelectedCards.Clear();

            SpawnPlayerHandItems();
        }

        public void OnCardSelected(Item item)
        {
            if (item == null || m_IsActionInProgress) return;
            if (!m_SelectedItems.Contains(item))
            {
                m_SelectedItems.Add(item); //why do we need both selecteditems and selected cards?
                m_SelectedCards.Add(new CardID(item.cardSuit, item.cardColour));   
                //m_SelectedColour = item.cardColour;                         //shouldnt change the colour based on the item?????
            }

            UpdateHeldItemsLayout();
        }

        public void OnCardDeselected(Item item)
        {
            if (item == null || m_IsActionInProgress) return;
            if (m_SelectedItems.Contains(item))
            {
                m_SelectedItems.Remove(item);
                m_SelectedCards.Remove(new CardID(item.cardSuit, item.cardColour));
                item.Deselect();
            }

            if (m_SelectedItems.Count > 0)
                m_SelectedColour = m_SelectedItems[0].cardColour;
            else if (m_Blackboard != null)
                m_SelectedColour = m_Blackboard.TargetColour;

            UpdateHeldItemsLayout();
        }

        private void UpdateHeldItemsLayout()
        {
            var count = m_SelectedItems.Count;
            var spacing = m_ItemSpacing * 0.6f;
            for (var i = 0; i < count; i++)
                m_SelectedItems[i].ApplySelectedVisuals(true, i, count, m_HeldItemContainer);
        }

        public void DeselectAllCards()
        {
            if (m_IsActionInProgress) return;
            m_SelectedCards.Clear();
            foreach (var item in m_SelectedItems)
            {
                if (item != null)
                    item.Deselect();
            }

            m_SelectedItems.Clear();
            if (m_Blackboard != null)
                m_SelectedColour = m_Blackboard.TargetColour;
        }

        public void OnCardAdded(CardColour colour, CardSuit suit)
            => m_SelectedCards.Add(new CardID(suit, colour));

        public void OnCardRemoved(CardColour colour, CardSuit suit)
            => m_SelectedCards.Remove(new CardID(suit, colour));

        public void OnColourSelected(CardColour colour)
        {
            m_SelectedColour = colour;

            switch (m_SelectedColour)
            {
                case CardColour.Red:
                    StartCoroutine(LerpColour(new Color(172 / 255f, 50 / 255f, 50 / 255f)));
                    break;
                case CardColour.Orange:
                    StartCoroutine(LerpColour(new Color(223 / 255f, 113 / 255f, 38 / 255f)));
                    break;
                case CardColour.Yellow:
                    StartCoroutine(LerpColour(new Color(251 / 255f, 242 / 255f, 54 / 255f)));
                    break;
                case CardColour.Green:
                    StartCoroutine(LerpColour(new Color(153 / 255f, 229 / 255f, 80 / 255f)));
                    break;
                case CardColour.Blue:
                    StartCoroutine(LerpColour(new Color(91 / 255f, 110 / 255f, 225 / 255f)));
                    break;
                case CardColour.Purple:
                    StartCoroutine(LerpColour(new Color(118 / 255f, 66 / 255f, 138 / 255f)));
                    break;
                case CardColour.Pink:
                    StartCoroutine(LerpColour(new Color(215 / 255f, 123 / 255f, 186 / 255f)));
                    break;
            }

        }

        private IEnumerator LerpColour(Color targetColour)
        {
            float elapsed = 0f;
            float duration = 1f;

            Color startColour = waterSpriteRenderer.color;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                float t = elapsed / duration;

                waterSpriteRenderer.color = Color.Lerp(startColour, targetColour, t);

                yield return null;
            }

            // Make sure we end exactly on the target colour
            waterSpriteRenderer.color = targetColour;
        }

        // Called when the player has finished playing the cards
        public void FinishPlay()
        {
            if (m_Fsm.CurrentState is PlayState playState && m_Blackboard.ActiveTurn == TurnUser.Player)
            {
                playState.CompletePlay();
                Debug.Log($"[GameManager] Player finished their play.");
            }
        }

        public void ConfirmPlayerDecision()
        {
            if (m_DialogueManager != null && m_DialogueManager.WasDialogueActiveRecently)
                return;

            if (m_IsActionInProgress)
                return;

            if (m_SelectedCards.Count == 0)
            {
                if (m_PlayerArmManager != null)
                    m_PlayerArmManager.ErrorJiggle();
                return;
            }

            if (m_Fsm.CurrentState is DecideState decideState && m_Blackboard.ActiveTurn == TurnUser.Player)
            {
                StartCoroutine(PlayerPlayRoutine(decideState));

            }
        }

        private IEnumerator PlayerPlayRoutine(DecideState decideState)
        {
            m_IsActionInProgress = true;

            if (m_PlayerArmManager != null)
                m_PlayerArmManager.RaiseArm();

            if (m_DialogueManager != null && m_PlayerAddedItemDialogue != null && m_PlayerAddedItemDialogue.Count > 0)
                m_DialogueManager.SetNewDialogue(m_PlayerAddedItemDialogue, m_SelectedCards.Count, m_SelectedColour);

            yield return new WaitForSeconds(0.1f);

            var playedCardsCopy = new List<CardID>(m_SelectedCards);
            var chosenColour = m_SelectedColour;

            RemovePlayedItemsDirectly();

            m_IsActionInProgress = false;

            decideState.ConfirmDecision(chosenColour, playedCardsCopy);

            SpawnPlayerHandItems();
            FinishPlay();
        }

        private void RemovePlayedItemsDirectly()
        {
            foreach (var item in m_SelectedItems)
            {
                if (item != null)
                {
                    m_SpawnedHandItems.Remove(item);
                    Destroy(item.gameObject);
                }
            }

            m_SelectedItems.Clear();
            m_SelectedCards.Clear();
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

        public void AIAddCards(int amount, CardColour colour)
        {
            OnColourSelected(colour);

            if (m_OpponentArmManager != null)
                m_OpponentArmManager.RaiseArm();
            if (m_DialogueManager != null)
                m_DialogueManager.SetNewDialogue(m_AIAddedItemDialogue, amount, colour);
        }

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
            m_IsActionInProgress = false;
            if (m_WinLossText != null)
                m_WinLossText.gameObject.SetActive(false);

            // Instantiates the round blackboard and FSM
            m_Blackboard = new GameBlackboard(m_AIProfile);
            m_SelectedColour = m_Blackboard.TargetColour;
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