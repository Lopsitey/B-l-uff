using System.Collections.Generic;
using Template.Content.Scripts.Card.Data;
using Template.Content.Scripts.Managers;
using UnityEngine;

namespace Template
{
    public class Item : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created

        public CardColour cardColour;
        public CardSuit cardSuit;
        private SpriteRenderer spriteRenderer;
        [SerializeField]private SpriteRenderer backSpriteRenderer;

        [SerializeField] private Sprite gemSprite;
        [SerializeField] private Sprite powderSprite;
        [SerializeField] private Sprite powderPaperSprite;
        [SerializeField] private Sprite plantStemSprite;
        [SerializeField] private Sprite flowerSprite;
        [SerializeField] private Sprite boneSprite;
        [SerializeField] private Sprite fleshSprite;

        private bool isSelected = false;

        private void OnMouseDown()
        {
            Debug.Log($"Clicked on {cardColour} {cardSuit} card.");

            if (!isSelected)
            {
                GameManager.Instance.OnCardAdded(cardColour, cardSuit);
                isSelected = true;
            }
            else
            {
                GameManager.Instance.OnCardRemoved(cardColour, cardSuit);
                isSelected = false;
            }

        }

        public void Initialize(CardColour colour, CardSuit suit)
        {
            cardColour = colour;
            cardSuit = suit;
            ApplyVisuals();
        }

        public void SetSortingOrder(int order)
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
                spriteRenderer.sortingOrder = order;

            if (backSpriteRenderer != null && spriteRenderer != null)
            {
                backSpriteRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
                backSpriteRenderer.sortingOrder = order - 1;
            }
        }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            ApplyVisuals();
        }

        public void ApplyVisuals()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            switch (cardColour)
            {
                case CardColour.Red:
                    spriteRenderer.material.color = Color.red;
                    break;
                case CardColour.Orange:
                    spriteRenderer.material.color = new Color(1f, 0.5f, 0f); // Orange
                    break;
                case CardColour.Yellow:
                    spriteRenderer.material.color = Color.yellow;
                    break;
                case CardColour.Green:
                    spriteRenderer.material.color = Color.green;
                    break;
                case CardColour.Blue:
                    spriteRenderer.material.color = Color.blue;
                    break;
                case CardColour.Purple:
                    spriteRenderer.material.color = new Color(0.5f, 0f, 0.5f); // Purple
                    break;
                case CardColour.Pink:
                    spriteRenderer.material.color = new Color(1f, 0.75f, 0.8f); // Pink
                    break;
            }

            if (backSpriteRenderer != null)
            {
                // Ensure back sprite is consistently rendered behind the front sprite
                backSpriteRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
                backSpriteRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;

                switch (cardSuit)
                {
                    case CardSuit.Gems:
                        spriteRenderer.sprite = gemSprite;
                        backSpriteRenderer.color = new Color(0.5f, 0.5f, 0.5f, 0f); // invis
                        break;
                    case CardSuit.Flesh:
                        spriteRenderer.sprite = fleshSprite;
                        backSpriteRenderer.sprite = boneSprite;
                        backSpriteRenderer.color = Color.white;
                        break;
                    case CardSuit.Flora:
                        spriteRenderer.sprite = flowerSprite;
                        backSpriteRenderer.sprite = plantStemSprite;
                        backSpriteRenderer.color = Color.white;
                        break;
                    case CardSuit.Vials:
                        spriteRenderer.sprite = powderSprite;
                        backSpriteRenderer.sprite = powderPaperSprite;
                        backSpriteRenderer.color = Color.white;
                        break;
                }
            }
        }

    }
}
