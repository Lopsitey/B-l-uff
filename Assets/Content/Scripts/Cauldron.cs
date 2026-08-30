using Template.Content.Scripts.Card.Data;
using Template.Content.Scripts.Managers;
using UnityEngine;

namespace Template
{
    public class Cauldron : MonoBehaviour
    {

        private void OnMouseDown()
        {
            if (GameManager.Instance != null && GameManager.Instance.m_DialogueManager != null && GameManager.Instance.m_DialogueManager.WasDialogueActiveRecently)
                return;

            if (GameManager.Instance != null && GameManager.Instance.IsActionInProgress)
                return;

            GameManager.Instance.ConfirmPlayerDecision();
        }
    }
}
