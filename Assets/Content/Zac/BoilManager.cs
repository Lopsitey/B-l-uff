using System;
using System.Collections.Generic;
using UnityEngine;

public class BoilManager : MonoBehaviour
{

    [SerializeField] private Dictionary<StatusEffectType, Boil> activeBoils = new();

    [SerializeField] private List<BoilEntry> boilEntries;


    [Serializable]
    public class BoilEntry
    {
        public StatusEffectType effectType;
        public Boil boil;
    }


    private Dictionary<StatusEffectType, Boil> boils;

    private void Awake()
    {
        boils = new Dictionary<StatusEffectType, Boil>();

        foreach (BoilEntry entry in boilEntries)
        {
            if (entry.boil == null)
                continue;

            boils[entry.effectType] = entry.boil;
        }
    }


    public void AddEffect(StatusEffectType effectType) //uses colour to add boil
    {
        if (boils.TryGetValue(effectType, out Boil boil))
        {
            boil.AddStack();
        }

    }

    public void DecrementEffects()
    {
        foreach (Boil effect in boils.Values)
        {
            effect.RemoveStack();

        }

    }

}
