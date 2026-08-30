using Template.Content.Scripts.Card.Data;
using Template.Content.Scripts.Managers;
using UnityEngine;

namespace Template
{
    public class Colour : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created

        public CardColour cardColour;

        private void OnMouseDown()
        {
            if (GameManager.Instance != null && GameManager.Instance.m_DialogueManager != null && GameManager.Instance.m_DialogueManager.WasDialogueActiveRecently)
                return;

            GameManager.Instance.OnColourSelected(cardColour);
            Debug.Log($"Selected {cardColour} colour.");
        }
    }
}
