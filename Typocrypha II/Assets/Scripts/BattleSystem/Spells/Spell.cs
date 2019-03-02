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
        caster.StartCoroutine(CastCR(caster, target));
        //Do post-cast visualization?
    }

    private IEnumerator CastCR(Caster caster, Battlefield.Position target)
    {
        foreach (var root in roots)
        {
            foreach (var effect in root.effects)
            {
                var targets = effect.pattern.Target(caster.FieldPos, target);
                foreach (var t in targets)
                {
                    var targetCaster = Battlefield.instance.GetCaster(t);
                    if (targetCaster == null)
                        Debug.Log("No target in targeted space: " + t);
                    else
                    {
                        effect.Cast(caster, targetCaster); //TODO: add effect logging?
                        yield return caster.StartCoroutine(DoSpellEffect(effect, t));
                        //Break if enemy killed?
                    }
                }
            }
        }
    }

    public IEnumerator DoSpellEffect(RootWordEffect effect, Battlefield.Position target)
    {
        switch (effect.effectType)
        {
            case RootWordEffect.EffectType.Single:
                yield return new WaitUntilAnimationComplete(
                    AnimationPlayer.instance.playAnimation(effect.effectPackets[0].clip, 
                    Battlefield.instance.GetSpace(target), 2));
                break;
            case RootWordEffect.EffectType.Sequence:
                foreach(var packet in effect.effectPackets)
                {
                    yield return new WaitUntilAnimationComplete(
                        AnimationPlayer.instance.playAnimation(packet.clip,
                        Battlefield.instance.GetSpace(target), 2));
                }
                break;
            case RootWordEffect.EffectType.Parallel:
                for(int i = 0; i < effect.effectPackets.Count; ++i)
                {
                    AnimationPlayer.instance.playAnimation(effect.effectPackets[i].clip, Battlefield.instance.GetSpace(target));
                }
                if(effect.effectPackets.Count > 0)
                    yield return new WaitUntilAnimationComplete(
                        AnimationPlayer.instance.playAnimation(effect.effectPackets[0].clip,
                        Battlefield.instance.GetSpace(target), 2));
                break;
            case RootWordEffect.EffectType.Prefab:
                if (effect.effectPrefab != null)
                {
                    var effectVisual = Object.Instantiate(effect.effectPrefab);
                    effectVisual.transform.position = Battlefield.instance.GetSpace(target);
                }
                break;
        }
    }
}
