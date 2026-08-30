using System.Collections;
using UnityEngine;

namespace Template
{
    public class ArmManager : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer heldItemSpriteRenderer;

        private Animator animator;
        private Coroutine m_JiggleCoroutine;
        private Vector3 m_OriginalPos;
        private bool m_HasOriginalPos;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void SetHeldItemSprite(Sprite sprite, Color color)
        {
            if (heldItemSpriteRenderer != null)
            {
                heldItemSpriteRenderer.sprite = sprite;
                heldItemSpriteRenderer.material.color = color;
            }
        }

        public void RaiseArm()
        {
            if (animator != null) animator.SetTrigger("Raise");
        }

        public void DropItem()
        {
            if (animator != null) animator.SetTrigger("Drop");
        }

        public void RevealItem()
        {
            if (animator != null) animator.SetTrigger("Reveal");
        }

        public void ErrorJiggle()
        {
            if (!m_HasOriginalPos)
            {
                m_OriginalPos = transform.localPosition;
                m_HasOriginalPos = true;
            }

            if (m_JiggleCoroutine != null)
                StopCoroutine(m_JiggleCoroutine);

            m_JiggleCoroutine = StartCoroutine(JiggleRoutine());
        }

        private IEnumerator JiggleRoutine()
        {
            var upPos = m_OriginalPos + new Vector3(0f, 0.75f, 0f);
            var duration = 0.12f;
            var elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                transform.localPosition = Vector3.Lerp(m_OriginalPos, upPos, elapsed / duration);
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                transform.localPosition = Vector3.Lerp(upPos, m_OriginalPos, elapsed / duration);
                yield return null;
            }

            transform.localPosition = m_OriginalPos;
            m_JiggleCoroutine = null;
        }
    }
}
