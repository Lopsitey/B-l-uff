using Template.Managers;
using UnityEngine;

namespace Template.UI.Models
{
    /// <summary>
    /// Plain data holder for settings. Not a MonoBehaviour.
    /// Stores values and saves/loads them. Controllers decide when to change values.
    /// </summary>
    public sealed class SettingsModel
    {
        private const string MasterVolumeKey = "settings.masterVolume";

        public float MasterVolume { get; private set; } = 0.5f;

        public void SetMasterVolume(float volume)
        {
            MasterVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat(MasterVolumeKey, MasterVolume);
            PlayerPrefs.Save();

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetMasterVolume(MasterVolume);
            }
        }

        public void Load()
        {
            MasterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, 0.5f);

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetMasterVolume(MasterVolume);
            }
        }
    }
}
