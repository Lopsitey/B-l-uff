using Template.Content.Scripts.Card.Data;
using Template.Content.Scripts.Card.Fsm.States;
using Template.Content.Scripts.Managers;
using Template.UI.Controllers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Template.Content.Scripts.Input
{
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
            m_Actions.UI.Click.performed += OnClickPerformed;
            m_Actions.UI.RightClick.performed += OnRightClickPerformed;

            m_Actions.Enable();
        }

        private void OnDisable()
        {
            m_Actions.UI.Cancel.performed -= OnCancelPerformed;
            m_Actions.UI.Click.performed -= OnClickPerformed;
            m_Actions.UI.RightClick.performed -= OnRightClickPerformed;

            m_Actions.Disable();
        }

        private void OnDestroy()
        {
            m_Actions.Dispose();
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.cKey.wasPressedThisFrame)
            {
                var gm = GameManager.Instance;
                if (gm != null && gm.m_PlayerArmManager != null)
                {
                    Debug.Log("[InputHandler] Debug 'C' pressed: Triggering Reveal on Player Arm");
                    gm.m_PlayerArmManager.RevealItem();
                }
            }
        }

        private void OnCancelPerformed(InputAction.CallbackContext context)
        {
            if (m_PauseMenu != null)
            {
                m_PauseMenu.TogglePause();
            }
        }

        private void OnClickPerformed(InputAction.CallbackContext context)
        {
            var gm = GameManager.Instance;
            if (gm == null) return;

            if (gm.m_DialogueManager != null && gm.m_DialogueManager.IsDialogueActive)
            {
                gm.m_DialogueManager.NextLine();
                return;
            }

            if (gm.CurrentState is ReactState && gm.ActiveTurn == TurnUser.Opponent)
            {
                gm.Pass();
            }
        }

        private void OnRightClickPerformed(InputAction.CallbackContext context)
        {
            var gm = GameManager.Instance;
            if (gm == null) return;

            // Allow right-click challenge even if dialogue is displaying the opponent's dialogue
            if (gm.CurrentState is ReactState && gm.ActiveTurn == TurnUser.Opponent)
            {
                gm.ChallengeOpponent();
                return;
            }

            if (gm.m_DialogueManager != null && gm.m_DialogueManager.IsDialogueActive)
                return;

            if (gm.CurrentState is DecideState && gm.ActiveTurn == TurnUser.Player)
            {
                gm.DeselectAllCards();
            }
        }
    }
}
