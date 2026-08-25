using UnityEngine;
using UnityEngine.UIElements;

namespace Template.UI.Views
{
    /// <summary>
    /// Queries the MainMenu UIDocument once and exposes the controls.
    /// No game logic here.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public sealed class MainMenuView : MonoBehaviour
    {
        public Button StartButton { get; private set; }
        public Button SettingsButton { get; private set; }
        public Button ControlsButton { get; private set; }
        public Button QuitButton { get; private set; }

        public Button SettingsBackButton { get; private set; }
        public Button ControlsBackButton { get; private set; }
        public Slider MasterVolumeSlider { get; private set; }

        public VisualElement MainPanel { get; private set; }
        public VisualElement SettingsPanel { get; private set; }
        public VisualElement ControlsPanel { get; private set; }

        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            MainPanel = root.Q<VisualElement>("MainPanel");
            SettingsPanel = root.Q<VisualElement>("SettingsPanel");
            ControlsPanel = root.Q<VisualElement>("ControlsPanel");

            StartButton = root.Q<Button>("StartButton");
            SettingsButton = root.Q<Button>("SettingsButton");
            ControlsButton = root.Q<Button>("ControlsButton");
            QuitButton = root.Q<Button>("QuitButton");

            SettingsBackButton = root.Q<Button>("SettingsBackButton");
            ControlsBackButton = root.Q<Button>("ControlsBackButton");
            MasterVolumeSlider = root.Q<Slider>("MasterVolumeSlider");
        }
    }
}
