using System.Collections;
using Template.Content.Scripts.Card.Data;
using Template.Content.Scripts.Card.Fsm.States;
using Template.Content.Scripts.Managers;
using UnityEngine;
using UnityEngine.Audio;

namespace Template
{
    public class Colour : MonoBehaviour
    {
        public CardColour cardColour;
        public SpriteRenderer waterSpriteRenderer;
        public AudioClip waterSound;

        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

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

            audioSource.pitch = Random.Range(0.7f, 1f);
            audioSource.PlayOneShot(waterSound, Random.Range(0.2f, 0.4f));

        }
    }
}
