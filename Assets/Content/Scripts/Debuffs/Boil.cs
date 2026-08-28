using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Boil : MonoBehaviour
{
    [Header("UI")]

    [SerializeField] private GameObject tooltip;
    [SerializeField] private TMP_Text effectNameText;
    [SerializeField] private TMP_Text descriptionText;

    [SerializeField] private SpriteRenderer boilSprite;


    [Header("Boil Properties")]
    public int StackCount = 1;
    [SerializeField] private StatusEffectDefinition effectStatus;

    public void AddStack()
    {
        showBoil();
        activateEffect();
        StackCount++;
    }

    public void RemoveStack()
    {
        if (StackCount == 0)
        {
            hideBoil();
            deactivateEffect();
            return;
        }
        StackCount--;
    }

    private void OnMouseEnter()
    {
        Debug.Log("Mouse entered boil!");

        tooltip.SetActive(true);

        effectNameText.text = effectStatus.DisplayName;
        descriptionText.text = effectStatus.Description;
    }

    private void OnMouseExit()
    {
        Debug.Log("Mouse left boil!");

        tooltip.SetActive(false);
    }

    private void showBoil()
    {
        boilSprite.enabled = true;
        boilSprite.GetComponent<SpriteRenderer>().enabled = true;

    }

    private void hideBoil()
    {
        boilSprite.enabled = false;

    }

    private void activateEffect()
    {
        switch (effectStatus.EffectType)
        {
            case StatusEffectType.MidasTouch:
                // Implement Midas Touch effect logic here
                break;
            case StatusEffectType.TruthSerum:
                // Implement Truth Serum effect logic here
                break;
            case StatusEffectType.ShakyHands:
                // Implement Shaky Hands effect logic here
                break;
            case StatusEffectType.StoneHands:
                // Implement Stone Hands effect logic here
                break;
            case StatusEffectType.ColourBlind:
                // Implement Colour Blind effect logic here
                break;
            case StatusEffectType.InvisibleHand:
                // Implement Invisible Hand effect logic here
                break;
            case StatusEffectType.LovePotion:
                // Implement Love Potion effect logic here
                break;
            case StatusEffectType.RavenousHunger:
                // Implement Ravenous Hunger effect logic here
                break;

        }
    }

    private void deactivateEffect()
    {


    }

}
