#region

using UnityEngine;
using UnityEngine.UIElements;

#endregion

namespace Template.Content.Scripts.UI.Views
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class GameHUDView : MonoBehaviour
    {
        public Label StateLabel { get; private set; }
        public Label InfoLabel { get; private set; }
        public Label ControlsLabel { get; private set; }

        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            StateLabel = root.Q<Label>("StateLabel");
            InfoLabel = root.Q<Label>("InfoLabel");
            ControlsLabel = root.Q<Label>("ControlsLabel");
        }

        public void UpdateHUD(string stateText, string infoText, string controlsText)
        {
            if (StateLabel != null) StateLabel.text = stateText;
            if (InfoLabel != null) InfoLabel.text = infoText;
            if (ControlsLabel != null) ControlsLabel.text = controlsText;
        }
    }
}