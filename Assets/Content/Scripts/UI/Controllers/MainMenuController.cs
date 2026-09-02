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
                m_View.MasterVolumeSlider.RegisterValueChangedCallback(OnMasterVolumeChanged);
            }

            ShowPanel(m_View.MainPanel);

            if (m_View.StartButton != null)
            {
                m_View.StartButton.clicked += OnStartClicked;
                m_View.StartButton.RegisterCallback<PointerEnterEvent>(OnButtonHover);
            }

            if (m_View.SettingsButton != null)
            {
                m_View.SettingsButton.clicked += OnSettingsClicked;
                m_View.SettingsButton.RegisterCallback<PointerEnterEvent>(OnButtonHover);
            }

            if (m_View.QuitButton != null)
            {
                m_View.QuitButton.clicked += OnQuitClicked;
                m_View.QuitButton.RegisterCallback<PointerEnterEvent>(OnButtonHover);
            }

            if (m_View.SettingsBackButton != null)
            {
                m_View.SettingsBackButton.clicked += OnBackToMainClicked;
                m_View.SettingsBackButton.RegisterCallback<PointerEnterEvent>(OnButtonHover);
            }

        }

        private void OnDestroy()
        {
            if (m_View == null)
            {
                return;
            }

            if (m_View.StartButton != null)
            {
                m_View.StartButton.clicked -= OnStartClicked;
                m_View.StartButton.UnregisterCallback<PointerEnterEvent>(OnButtonHover);
            }

            if (m_View.SettingsButton != null)
            {
                m_View.SettingsButton.clicked -= OnSettingsClicked;
                m_View.SettingsButton.UnregisterCallback<PointerEnterEvent>(OnButtonHover);
            }

            if (m_View.QuitButton != null)
            {
                m_View.QuitButton.clicked -= OnQuitClicked;
                m_View.QuitButton.UnregisterCallback<PointerEnterEvent>(OnButtonHover);
            }

            if (m_View.SettingsBackButton != null)
            {
                m_View.SettingsBackButton.clicked -= OnBackToMainClicked;
                m_View.SettingsBackButton.UnregisterCallback<PointerEnterEvent>(OnButtonHover);
            }
        }

        private void ShowPanel(VisualElement panelToShow)
        {
            SetPanelVisible(m_View.MainPanel, panelToShow == m_View.MainPanel);
            SetPanelVisible(m_View.SettingsPanel, panelToShow == m_View.SettingsPanel);
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

        private void OnButtonHover(PointerEnterEvent evt)
        {
            AudioManager.Instance?.PlayUIHover();
        }

        private IEnumerator StartupDelay(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            m_cameraCutscene.SetActive(true);
            yield return new WaitForSeconds(seconds);
            m_dialogueManager.GetComponent<DialogueManager>().StartDialogue();
            yield return new WaitForSeconds(71f);                            
            SceneManager.LoadScene("CardRound");
        }

        private void OnSettingsClicked()
        {

            AudioManager.Instance?.PlayUiClick();
            ShowPanel(m_View.SettingsPanel);
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
