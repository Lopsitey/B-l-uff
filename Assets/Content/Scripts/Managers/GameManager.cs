#region

using System.Collections.Generic;
using Template.Content.Scripts.Card.Blackboard;
using Template.Content.Scripts.Card.Data;
using Template.Content.Scripts.Card.Fsm;
using Template.Content.Scripts.Card.Fsm.States;
using Template.Content.Scripts.Card.Fuzzy;
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

        public bool LastPlayWasChallenged => m_Blackboard.LastPlayWasChallenged;

        public ICardRoundState CurrentState => m_Fsm?.CurrentState;

        private GameBlackboard m_Blackboard;
        private FSM m_Fsm;

        public DialogueManager DialogueManager;

        [SerializeField] private List<DialogueLine> introDialogue;
        [SerializeField] private List<DialogueLine> addItemDialogue;
        [SerializeField] private List<DialogueLine> AIaddItemDialogue;
        [SerializeField] private List<DialogueLine> callOutWrongDialogue;
        [SerializeField] private List<DialogueLine> callOutCorrectDialogue;
        [SerializeField] private List<DialogueLine> getCalledOutWrongDialogue;
        [SerializeField] private List<DialogueLine> getCalledOutCorrectDialogue;
        [SerializeField] private List<DialogueLine> winDialogue;
        [SerializeField] private List<DialogueLine> loseDialogue;

        // Called by the UI when the player has made their decision and is ready to play cards

        private List<CardID> m_SelectedCards = new List<CardID>();
        private CardColour m_SelectedColour;

        public void OnCardAdded(CardColour colour, CardSuit suit)
        {
            m_SelectedCards.Add(new CardID(suit, colour));
        }

        public void OnCardRemoved(CardColour colour, CardSuit suit)
        {
            m_SelectedCards.Remove(new CardID(suit, colour));
        }

        public void OnColourSelected(CardColour colour)
        {
            m_SelectedColour = colour;
        }

        // Called when the player has finished playing the cards
        public void FinishPlay()
        {
            if (m_Fsm.CurrentState is PlayState playState && m_Blackboard.ActiveTurn == TurnUser.Player)
            {
                playState.CompletePlay();
                Debug.Log($"[GameManager] Player finished their play with {m_SelectedCards.Count} cards.");
                DialogueManager.SetNewDialogue(addItemDialogue);
            }
            // Can also call player finish play dialogue here - adding an item etc

        }

        public void ConfirmPlayerDecision()
        {
            if (m_Fsm.CurrentState is DecideState decideState && m_Blackboard.ActiveTurn == TurnUser.Player)
            {
                decideState.ConfirmDecision(m_SelectedColour, m_SelectedCards);
            }
        }

        public void ChallengeOpponent() 
        {
            if (m_Fsm.CurrentState is ReactState reactState && m_Blackboard.ActiveTurn == TurnUser.Opponent)
                reactState.Challenge();
            // Can also call player challenge dialogue here
        }

        public void Pass()
        {
            if (m_Fsm.CurrentState is ReactState reactState && m_Blackboard.ActiveTurn == TurnUser.Opponent)
                reactState.Pass();
        }


        public void ChallengeOpponentCorrect()
        {
            DialogueManager.SetNewDialogue(callOutCorrectDialogue);
        }

        public void ChallengeOpponentWrong()
        {
            DialogueManager.SetNewDialogue(callOutWrongDialogue);
        }


        public void AIDialogueChallenge()
        {
            //not needed?
        }

        public void AIDialogueChallengeCorrect()
        {
            DialogueManager.SetNewDialogue(getCalledOutCorrectDialogue);
        }

        public void AIDialogueChallengeWrong()
        {
            DialogueManager.SetNewDialogue(getCalledOutWrongDialogue);
        }

        public void AIAddCards()
        {
            DialogueManager.SetNewDialogue(AIaddItemDialogue);
        }

        public void EndDialogue(bool playerWon)
        {
            // Called when the player has ended the round
            if (playerWon) //Use for win/loss dialogue
            {
                DialogueManager.SetNewDialogue(winDialogue);
                Debug.Log($"[GameManager] Player won the round vs {m_AIProfile.DisplayName}.");
            }
            else
            {
                DialogueManager.SetNewDialogue(loseDialogue);       
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

            DialogueManager.SetNewDialogue(introDialogue);
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