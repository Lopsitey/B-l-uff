#region

using Template.Content.Scripts.Managers;
using UnityEngine;

#endregion

namespace Template.Managers
{
    public sealed class AudioManager : Singleton<AudioManager>
    {
        [Header("Volume (0 to 1)")] [SerializeField] [Range(0f, 1f)]
        private float m_MasterVolume = 0.5f;

        [Header("Optional UI clip")] [SerializeField]
        private AudioClip m_UiClickClip;

        private AudioSource m_SfxSource;

        protected override void Awake()
        {
            base.Awake();
            m_SfxSource = GetComponent<AudioSource>();
        }

        public void PlayUiClick()
        {
            PlaySfx(m_UiClickClip, 0.8f);
        }

        public void PlaySfx(AudioClip clip, float volumeMultiplier = 1f)
        {
            if (clip == null || m_SfxSource == null)
            {
                return;
            }

            m_SfxSource.PlayOneShot(clip, m_MasterVolume * volumeMultiplier);
        }

        public void SetMasterVolume(float volume)
        {
            m_MasterVolume = Mathf.Clamp01(volume);
        }
    }
}