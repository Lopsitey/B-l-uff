using UnityEngine;
using UnityEngine.UIElements;

namespace Template.UI.Views
{
    /// <summary>
    /// Queries the PauseMenu UIDocument once and exposes the controls.
    /// No game logic here.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public sealed class PauseMenuView : MonoBehaviour
    {
        public VisualElement RootOverlay { get; private set; }
        public VisualElement PausePanel { get; private set; }
        public VisualElement SettingsPanel { get; private set; }
        public VisualElement ControlsPanel { get; private set; }

        public Button ResumeButton { get; private set; }
        public Button SettingsButton { get; private set; }
        public Button ControlsButton { get; private set; }
        public Button MainMenuButton { get; private set; }
        public Button QuitButton { get; private set; }

        public Button SettingsBackButton { get; private set; }
        public Button ControlsBackButton { get; private set; }
        public Slider MasterVolumeSlider { get; private set; }

        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            RootOverlay = root.Q<VisualElement>("PauseRoot");
            PausePanel = root.Q<VisualElement>("PausePanel");
            SettingsPanel = root.Q<VisualElement>("PauseSettingsPanel");
            ControlsPanel = root.Q<VisualElement>("PauseControlsPanel");

            ResumeButton = root.Q<Button>("ResumeButton");
            SettingsButton = root.Q<Button>("PauseSettingsButton");
            ControlsButton = root.Q<Button>("PauseControlsButton");
            MainMenuButton = root.Q<Button>("MainMenuButton");
            QuitButton = root.Q<Button>("PauseQuitButton");

            SettingsBackButton = root.Q<Button>("PauseSettingsBackButton");
            ControlsBackButton = root.Q<Button>("PauseControlsBackButton");
            MasterVolumeSlider = root.Q<Slider>("PauseMasterVolumeSlider");

            if (RootOverlay != null)
            {
                RootOverlay.style.display = DisplayStyle.None;
            }
        }

        public void SetVisible(bool visible)
        {
            if (RootOverlay == null)
            {
                return;
            }

            RootOverlay.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
