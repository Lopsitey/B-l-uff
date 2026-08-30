#region

using System.Collections.Generic;
using Template.Content.Scripts.Card.Data;
using Template.Content.Scripts.Card.Fsm.States;
using Template.Content.Scripts.Managers;
using UnityEngine;

#endregion

namespace Template.Content.Scripts
{
    public class Item : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created

        public CardColour cardColour;
        public CardSuit cardSuit;
        private SpriteRenderer spriteRenderer;
        [SerializeField] private SpriteRenderer backSpriteRenderer;

        [SerializeField] private Sprite gemSprite;
        [SerializeField] private Sprite powderSprite;
        [SerializeField] private Sprite powderPaperSprite;
        [SerializeField] private Sprite plantStemSprite;
        [SerializeField] private Sprite flowerSprite;
        [SerializeField] private Sprite boneSprite;
        [SerializeField] private Sprite fleshSprite;

        private bool isSelected = false;
        private Transform m_HandParent;
        private Vector3 m_HandLocalPosition;
        private int m_HandSortingOrder;

        public void SetHandOrigin(Transform handParent, Vector3 localPos, int sortingOrder)
        {
            m_HandParent = handParent;
            m_HandLocalPosition = localPos;
            m_HandSortingOrder = sortingOrder;
            SetSortingOrder(sortingOrder);
        }

        public void ApplySelectedVisuals(bool selected, int selectedIndex = 0, int totalSelected = 1,
            Transform heldContainer = null, float spacing = 1.5f)
        {
            isSelected = selected;
            if (selected)
            {
                if (heldContainer != null)
                {
                    transform.SetParent(heldContainer);
                    var startX = -((totalSelected - 1) * spacing) / 2f;
                    transform.localPosition = new Vector3(startX + (selectedIndex * spacing), 0f, 0f);
                }

                SetGreyscaleVisuals(true);
                SetSortingOrder(100 + (selectedIndex * 2) + 1);
            }
            else
            {
                if (m_HandParent != null)
                    transform.SetParent(m_HandParent);
                transform.localPosition = m_HandLocalPosition;
                SetGreyscaleVisuals(false);
                SetSortingOrder(m_HandSortingOrder);
            }
        }

        private void SetGreyscaleVisuals(bool greyed)
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            var tint = greyed ? new Color(0.55f, 0.55f, 0.55f, 1f) : Color.white;
            if (spriteRenderer != null)
                spriteRenderer.color = tint;

            if (backSpriteRenderer != null && backSpriteRenderer.sprite != null)
                backSpriteRenderer.color = tint;
        }

        public void Deselect()
        {
            isSelected = false;
            ApplySelectedVisuals(false);
        }

        private void OnMouseDown()
        {
            if (GameManager.Instance != null && GameManager.Instance.m_DialogueManager != null &&
                GameManager.Instance.m_DialogueManager.WasDialogueActiveRecently)
                return;

            if (GameManager.Instance == null ||
                GameManager.Instance.IsActionInProgress ||
                GameManager.Instance.CurrentState is not DecideState ||
                GameManager.Instance.ActiveTurn != TurnUser.Player)
                return;

            Debug.Log($"Clicked on {cardColour} {cardSuit} card.");

            if (!isSelected)
            {
                isSelected = true;
                GameManager.Instance.OnCardSelected(this);
            }
            else
            {
                isSelected = false;
                GameManager.Instance.OnCardDeselected(this);
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

            switch (cardSuit)
            {
                case CardSuit.Gems:
                    spriteRenderer.sprite = gemSprite;
                    if (backSpriteRenderer != null)
                    {
                        backSpriteRenderer.sprite = null;
                        backSpriteRenderer.color = new Color(0.5f, 0.5f, 0.5f, 0f); // invis
                    }

                    break;
                case CardSuit.Flesh:
                    spriteRenderer.sprite = fleshSprite;
                    if (backSpriteRenderer != null)
                    {
                        backSpriteRenderer.sprite = boneSprite;
                        backSpriteRenderer.color = Color.white;
                    }

                    break;
                case CardSuit.Flora:
                    spriteRenderer.sprite = flowerSprite;
                    if (backSpriteRenderer != null)
                    {
                        backSpriteRenderer.sprite = plantStemSprite;
                        backSpriteRenderer.color = Color.white;
                    }

                    break;
                case CardSuit.Vials:
                    spriteRenderer.sprite = powderSprite;
                    if (backSpriteRenderer != null)
                    {
                        backSpriteRenderer.sprite = powderPaperSprite;
                        backSpriteRenderer.color = Color.white;
                    }

                    break;
            }

            UpdatePhysicsCollider();
        }

        private void UpdatePhysicsCollider()
        {
            var polyCol = GetComponent<PolygonCollider2D>();
            if (polyCol == null)
                polyCol = gameObject.AddComponent<PolygonCollider2D>();

            var boxCol = GetComponent<BoxCollider2D>();
            if (boxCol != null)
                Destroy(boxCol);

            // For Vials (powder), use only the paper sprite's physics shape to avoid subtractive overlapping cutouts
            var sprite = cardSuit == CardSuit.Vials ? null : (spriteRenderer != null ? spriteRenderer.sprite : null);
            var backSprite = (backSpriteRenderer != null && backSpriteRenderer.color.a > 0.01f)
                ? backSpriteRenderer.sprite
                : null;

            var pathCount = 0;
            if (sprite != null) pathCount += sprite.GetPhysicsShapeCount();
            if (backSprite != null) pathCount += backSprite.GetPhysicsShapeCount();

            polyCol.pathCount = pathCount;
            var currentPathIndex = 0;
            var shapePoints = new List<Vector2>();

            if (sprite != null)
            {
                for (var i = 0; i < sprite.GetPhysicsShapeCount(); i++)
                {
                    shapePoints.Clear();
                    sprite.GetPhysicsShape(i, shapePoints);
                    polyCol.SetPath(currentPathIndex++, shapePoints);
                }
            }

            if (backSprite != null)
            {
                for (var i = 0; i < backSprite.GetPhysicsShapeCount(); i++)
                {
                    shapePoints.Clear();
                    backSprite.GetPhysicsShape(i, shapePoints);
                    polyCol.SetPath(currentPathIndex++, shapePoints);
                }
            }
        }
    }
}