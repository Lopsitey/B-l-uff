using System.Collections;
using UnityEngine;

namespace Template
{
    public class ArmManager : MonoBehaviour
    {

        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();

        }


        public void SetHeldItemSprite(Sprite sprite, Color color)
        {

        }

        public void RaiseArm()
        {
            if (animator != null) animator.SetTrigger("Raise");

            Debug.Log($"[ArmManager] RaiseArm");
        }

        public void DropItem()
        {
            if (animator != null) animator.SetTrigger("Drop");

            Debug.Log($"[ArmManager] DropItem");
        }

        public void RevealItem()
        {
            if (animator != null) animator.SetTrigger("Reveal");

            Debug.Log($"[ArmManager] RevealItem");
        }

    }
}
