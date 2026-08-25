using Template.UI.Controllers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Template.Input
{
    /// <summary>
    /// Input System bridge. Keeps pause handling in one place.
    /// </summary>
    public sealed class InputHandler : MonoBehaviour
    {
        private InputSystem_Actions m_Actions;
        private PauseMenuController m_PauseMenu;

        private void Awake()
        {
            m_Actions = new InputSystem_Actions();
            m_PauseMenu = GetComponent<PauseMenuController>();
        }

        private void OnEnable()
        {
            m_Actions.UI.Cancel.performed += OnCancelPerformed;
            m_Actions.Enable();
        }

        private void OnDisable()
        {
            m_Actions.UI.Cancel.performed -= OnCancelPerformed;
            m_Actions.Disable();
        }

        private void OnCancelPerformed(InputAction.CallbackContext context)
        {
            if (m_PauseMenu == null)
            {
                return;
            }

            m_PauseMenu.TogglePause();
        }
    }
}
