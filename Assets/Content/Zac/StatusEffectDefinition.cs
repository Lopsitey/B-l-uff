using UnityEngine;

public enum StatusEffectType
{
    None,
    // Current game effects
    MidasTouch,
    TruthSerum,
    ShakyHands,
    StoneHands,
    ColourBlind,
    InvisibleHand,
    LovePotion,
    RavenousHunger,

    // Future effects
    InvertColour,
    ChangeColour
}

[CreateAssetMenu(fileName = "StatusEffectDefinition", menuName = "Scriptable Objects/StatusEffectDefinition")]


public class StatusEffectDefinition : ScriptableObject
{

    [Header("Identity")]
    [SerializeField] private StatusEffectType effectType;
    [SerializeField] private string displayName;

    [TextArea(2, 5)]
    [SerializeField] private string description;

    [Header("Visual")]
    [SerializeField] private Sprite boilIcon;

    [Header("Behaviour")]
    [SerializeField] private int defaultDuration = -1;
    [SerializeField] private bool stackable;

    public StatusEffectType EffectType => effectType;
    public string DisplayName => displayName;
    public string Description => description;
    public Sprite BoilIcon => boilIcon;
    public int DefaultDuration => defaultDuration;
    public bool Stackable => stackable;

}
