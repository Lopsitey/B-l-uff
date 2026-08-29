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

        public void RaiseArm()
        {
            animator.SetTrigger("Raise");
        
        }

        public void DropItem()
        {
            animator.SetTrigger("Drop");

        }

        public void RevealItem()
        {
            animator.SetTrigger("Reveal");
        }
    }
}
