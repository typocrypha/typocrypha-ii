using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Spell
{
    // Spell tag dictionary
    private RootWord[] roots;
    // Accessible spelleffects (already modified or list of modifications)
    
    /// <summary> Create a castable spell out of a chain of spell words. Assumes the chain is valid </summary>
    public Spell(SpellWord[] words)
    {
        //DEBUG: just grab all the root words
        roots = (words.Where((word) => word is RootWord).Select((word) => word as RootWord)).ToArray();
        //TODO: Search for modifiers. apply modifiers
    }

    //TODO: add waiting/coroutine for spell visualization
    /// <summary> Cast the spell's effect with a given caster at a given target position </summary>
    public void Cast (Caster caster, Battlefield.Position target)
    {
        foreach(var root in roots)
        {
            foreach(var effect in root.effects)
            {
                var targets = effect.pattern.Target(caster.FieldPos, target);
                foreach(var t in targets)
                {
                    var targetCaster = Battlefield.instance.GetCaster(t);
                    if (targetCaster == null)
                        Debug.Log("No target in targeted space: " + t);
                    else
                    {
                        effect.Cast(caster, targetCaster); //TODO: add effect logging?
                        if(effect.effect != null)
                        {
                            var effectVisual = Object.Instantiate(effect.effect);
                            effectVisual.transform.position = Battlefield.instance.GetSpace(t);
                        }                           
                        //Break if enemy killed?
                    }
                }
            }
        }
        //Do post-cast visualization?
    }
}
