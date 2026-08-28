#region

using UnityEngine;

#endregion

namespace Template.Content.Scripts.Card.Fuzzy
{
    [CreateAssetMenu(fileName = "AIFuzzyProfile", menuName = "Bluff/Opponent Fuzzy Profile")]
    public sealed class AIFuzzyProfile : ScriptableObject
    {
        [SerializeField] private string m_DisplayName;

        [Header("Trust")] [SerializeField] [Range(0f, 1f)]
        private float m_StartingTrust = 0.5f;

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("How much trust is lost when the opponent catches the player in a lie.")]
        private float m_DistrustPerLie = 0.1f;

        [Header("Play Style")]
        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip(
            "How willing the opponent is to bluff. Higher = more likely to bluff, lower = more likely to play honestly.")]
        private float m_LieChance = 0.3f;

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("Willingness to call the player's bluff. Higher = more likely to call, lower = more likely to fold.")]
        private float m_CallChance = 0.5f;

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip(
            "Willingness to play multiple cards. Honest plays scale with combo strength in hand. Lies use this value directly.")]
        private float m_ComboChance = 0.3f;

        public string DisplayName => string.IsNullOrEmpty(m_DisplayName) ? name : m_DisplayName;
        public float StartingTrust => m_StartingTrust;
        public float DistrustPerLie => m_DistrustPerLie;
        public float CallChance => m_CallChance;
        public float LieChance => m_LieChance;
        public float ComboChance => m_ComboChance;
    }
}