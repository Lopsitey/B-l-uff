using System.Collections;
using Template.Managers;
using Template.UI.Models;
using Template.UI.Views;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Template.UI.Controllers
{
    /// <summary>
    /// Handles main menu button presses and panel swaps.
    /// SettingsModel is injected through the constructor-style Awake setup (plain new + Load).
    /// </summary>
    [RequireComponent(typeof(MainMenuView))]
    public sealed class MainMenuController : MonoBehaviour
    {
        private MainMenuView m_View;
        private SettingsModel m_Settings;
        [SerializeField] private GameObject m_cameraCutscene;
        [SerializeField] private GameObject m_dialogueManager;

        private void Awake()
        {
            m_View = GetComponent<MainMenuView>();

            // Dependency injection for a tiny system: create the model and hand it to this controller.
            // Expand later with a real injector if the project grows.
            m_Settings = new SettingsModel();
            m_Settings.Load();
        }

        private void Start()
        {
            if (m_View.MasterVolumeSlider != null)
            {
                m_View.MasterVolumeSlider.value = m_Settings.MasterVolume;
            }

            ShowPanel(m_View.MainPanel);

            m_View.StartButton.clicked += OnStartClicked;
            m_View.SettingsButton.clicked += OnSettingsClicked;
            m_View.ControlsButton.clicked += OnControlsClicked;
            m_View.QuitButton.clicked += OnQuitClicked;
            m_View.SettingsBackButton.clicked += OnBackToMainClicked;
            m_View.ControlsBackButton.clicked += OnBackToMainClicked;
            m_View.MasterVolumeSlider.RegisterValueChangedCallback(OnMasterVolumeChanged);
        }

        private void OnDestroy()
        {
            if (m_View == null)
            {
                return;
            }

            m_View.StartButton.clicked -= OnStartClicked;
            m_View.SettingsButton.clicked -= OnSettingsClicked;
            m_View.ControlsButton.clicked -= OnControlsClicked;
            m_View.QuitButton.clicked -= OnQuitClicked;
            m_View.SettingsBackButton.clicked -= OnBackToMainClicked;
            m_View.ControlsBackButton.clicked -= OnBackToMainClicked;
        }

        private void ShowPanel(VisualElement panelToShow)
        {
            SetPanelVisible(m_View.MainPanel, panelToShow == m_View.MainPanel);
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

        private void OnStartClicked()
        {
            AudioManager.Instance?.PlayUiClick();
            gameObject.GetComponent<UIDocument>().enabled = false;

            StartCoroutine(StartupDelay(1f));
        }

        private IEnumerator StartupDelay(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            m_cameraCutscene.SetActive(true);
            yield return new WaitForSeconds(seconds);
            m_dialogueManager.SetActive(true);
            yield return new WaitForSeconds(73f);
            SceneManager.LoadScene("CardRound");
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

        private void OnBackToMainClicked()
        {
            AudioManager.Instance?.PlayUiClick();
            ShowPanel(m_View.MainPanel);
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
