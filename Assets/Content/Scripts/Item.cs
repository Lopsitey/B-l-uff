using Microsoft.CodeAnalysis;
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

        void Start()
        {
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


            //switch (cardSuit)
            //{
            //    case CardSuit.Gems:
            //        spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Gem");
            //        break;
            //    case CardSuit.Flesh:
            //        spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Flesh");
            //        break;
            //    case CardSuit.Flora:
            //        spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Flora");
            //        break;
            //    case CardSuit.Vials:
            //        spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Vial");
            //        break;
            //}

        }

    }
}
