using Template.Content.Scripts.Card.Data;
using Template.Content.Scripts.Card.Fsm.States;
using Template.Content.Scripts.Managers;
using UnityEngine;

namespace Template
{
    public class Colour : MonoBehaviour
    {
        public CardColour cardColour;

        private void OnMouseDown()
        {
            if (GameManager.Instance != null && GameManager.Instance.m_DialogueManager != null && GameManager.Instance.m_DialogueManager.WasDialogueActiveRecently)
                return;

            if (GameManager.Instance == null ||
                GameManager.Instance.IsActionInProgress ||
                GameManager.Instance.CurrentState is not DecideState ||
                GameManager.Instance.ActiveTurn != TurnUser.Player)
                return;

            GameManager.Instance.OnColourSelected(cardColour);
            Debug.Log($"Selected {cardColour} colour.");
        }
    }
}
