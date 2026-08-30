using System.Collections;
using UnityEngine;

namespace Template
{
    public class ArmManager : MonoBehaviour
    {
        [SerializeField] private Sprite hand0;
        [SerializeField] private Sprite hand1;
        [SerializeField] private Sprite hand2;

        private SpriteRenderer m_ArmSpriteRenderer;
        private Animator animator;
        private Coroutine m_JiggleCoroutine;
        private Vector3 m_OriginalPos;
        private bool m_HasOriginalPos;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            m_ArmSpriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetHandSprite(int index)
        {
            if (m_ArmSpriteRenderer == null)
                m_ArmSpriteRenderer = GetComponent<SpriteRenderer>();

            if (m_ArmSpriteRenderer != null)
            {
                if (index == 0 && hand0 != null) m_ArmSpriteRenderer.sprite = hand0;
                else if (index == 1 && hand1 != null) m_ArmSpriteRenderer.sprite = hand1;
                else if (index == 2 && hand2 != null) m_ArmSpriteRenderer.sprite = hand2;
            }
        }

        public void SetHeldItemSprite(Sprite sprite, Color color)
        {
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
            var startPos = transform.localPosition;

            if (m_JiggleCoroutine != null)
                StopCoroutine(m_JiggleCoroutine);

            m_JiggleCoroutine = StartCoroutine(JiggleRoutine(startPos));
        }

        private IEnumerator JiggleRoutine(Vector3 startPos)
        {
            var upPos = startPos + new Vector3(0f, 0.75f, 0f);
            var duration = 0.12f;
            var elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                transform.localPosition = Vector3.Lerp(startPos, upPos, elapsed / duration);
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                transform.localPosition = Vector3.Lerp(upPos, startPos, elapsed / duration);
                yield return null;
            }

            transform.localPosition = startPos;
            m_JiggleCoroutine = null;
        }
    }
}
