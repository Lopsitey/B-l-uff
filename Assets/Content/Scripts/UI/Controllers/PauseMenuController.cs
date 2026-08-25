using Template.Core;
using Template.Managers;
using Template.UI.Models;
using Template.UI.Views;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Template.UI.Controllers
{
    /// <summary>
    /// Handles pause overlay button presses and panel swaps.
    /// </summary>
    public sealed class PauseMenuController : MonoBehaviour
    {
        [SerializeField]
        private PauseMenuView m_View;

        private SettingsModel m_Settings;
        private bool m_IsOpen;

        private void Awake()
        {
            if (m_View == null)
            {
                m_View = GetComponent<PauseMenuView>();
            }

            m_Settings = new SettingsModel();
            m_Settings.Load();
        }

        private void Start()
        {
            if (m_View.MasterVolumeSlider != null)
            {
                m_View.MasterVolumeSlider.value = m_Settings.MasterVolume;
            }

            m_View.ResumeButton.clicked += OnResumeClicked;
            m_View.SettingsButton.clicked += OnSettingsClicked;
            m_View.ControlsButton.clicked += OnControlsClicked;
            m_View.MainMenuButton.clicked += OnMainMenuClicked;
            m_View.QuitButton.clicked += OnQuitClicked;
            m_View.SettingsBackButton.clicked += OnBackToPauseClicked;
            m_View.ControlsBackButton.clicked += OnBackToPauseClicked;
            m_View.MasterVolumeSlider.RegisterValueChangedCallback(OnMasterVolumeChanged);
        }

        private void OnDestroy()
        {
            if (m_View == null)
            {
                return;
            }

            m_View.ResumeButton.clicked -= OnResumeClicked;
            m_View.SettingsButton.clicked -= OnSettingsClicked;
            m_View.ControlsButton.clicked -= OnControlsClicked;
            m_View.MainMenuButton.clicked -= OnMainMenuClicked;
            m_View.QuitButton.clicked -= OnQuitClicked;
            m_View.SettingsBackButton.clicked -= OnBackToPauseClicked;
            m_View.ControlsBackButton.clicked -= OnBackToPauseClicked;
        }

        public void TogglePause()
        {
            if (m_IsOpen)
            {
                ClosePause();
            }
            else
            {
                OpenPause();
            }
        }

        public void OpenPause()
        {
            m_IsOpen = true;
            ShowPanel(m_View.PausePanel);
            m_View.SetVisible(true);
            GameManager.Instance?.SetPaused(true);
        }

        public void ClosePause()
        {
            m_IsOpen = false;
            m_View.SetVisible(false);
            GameManager.Instance?.SetPaused(false);
        }

        private void ShowPanel(VisualElement panelToShow)
        {
            SetPanelVisible(m_View.PausePanel, panelToShow == m_View.PausePanel);
            SetPanelVisible(m_View.SettingsPanel, panelToShow == m_View.SettingsPanel);
            SetPanelVisible(m_View.ControlsPanel, panelToShow == m_View.ControlsPanel);
        }

        private static void SetPanelVisible(VisualElement panel, bool visible)
        {
            if (panel == null)
            {
                return;
            }

            panel.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void OnResumeClicked()
        {
            AudioManager.Instance?.PlayUiClick();
            ClosePause();
        }

        private void OnSettingsClicked()
        {
            AudioManager.Instance?.PlayUiClick();
            ShowPanel(m_View.SettingsPanel);
        }

        private void OnControlsClicked()
        {
            AudioManager.Instance?.PlayUiClick();
            ShowPanel(m_View.ControlsPanel);
        }

        private void OnBackToPauseClicked()
        {
            AudioManager.Instance?.PlayUiClick();
            ShowPanel(m_View.PausePanel);
        }

        private void OnMainMenuClicked()
        {
            AudioManager.Instance?.PlayUiClick();
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneNames.MainMenu);
        }

        private void OnQuitClicked()
        {
            AudioManager.Instance?.PlayUiClick();
            Application.Quit();
        }

        private void OnMasterVolumeChanged(ChangeEvent<float> changeEvent)
        {
            m_Settings.SetMasterVolume(changeEvent.newValue);
        }
    }
}
