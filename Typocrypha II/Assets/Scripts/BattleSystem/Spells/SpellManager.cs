using System.Collections;
using System.Linq;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    private const float delayBetweenTargets = 0.1f;
    public static SpellManager instance;
    public GameObject noTargetFxPrefab;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary> Cast the spell's effect with a given caster at a given target position </summary>
    public void Cast (SpellWord[] words, Caster caster, Battlefield.Position target)
    {
        StartCoroutine(CastCR(Modify(words), caster, target));
    }
    //DEBUG: Should probably eventually generate some sort of aggregate spell data
    private RootWord[] Modify(SpellWord[] words)
    {
        //DEBUG: just grab all the root words
        //TODO: Search for modifiers. apply modifiers
        return (words.Where((word) => word is RootWord).Select((word) => word as RootWord)).ToArray();
    }
    
    private IEnumerator CastCR(RootWord[] roots, Caster caster, Battlefield.Position target)
    {
        foreach (var root in roots)
        {
            foreach (var effect in root.effects)
            {
                var targets = effect.pattern.Target(caster.FieldPos, target);
                Coroutine spellCR = null;
                foreach (var t in targets)
                {
                    var targetCaster = Battlefield.instance.GetCaster(t);                   
                    if (targetCaster == null)
                    {
                        var noTargetEffect = Instantiate(noTargetFxPrefab);
                        noTargetEffect.transform.position = Battlefield.instance.GetSpace(t);
                        var fxComponent = noTargetEffect.GetComponent<SpellFx>();
                        if (fxComponent != null)
                            spellCR = fxComponent.StartCoroutine(fxComponent.PlayEffect());
                    }
                    else
                    {
                        effect.Cast(caster, targetCaster); //TODO: add effect logging?
                        spellCR = StartCoroutine(DoSpellEffect(effect, t));
                        yield return new WaitForSeconds(delayBetweenTargets);
                        //Break if enemy killed?
                    }                 
                }
                if(spellCR != null)
                    yield return spellCR;
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
                    Battlefield.instance.GetSpace(target), 1f));
                break;
            case RootWordEffect.EffectType.Sequence:
                foreach(var packet in effect.effectPackets)
                {
                    yield return new WaitUntilAnimationComplete(
                        AnimationPlayer.instance.playAnimation(packet.clip,
                        Battlefield.instance.GetSpace(target), 1f));
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
                        Battlefield.instance.GetSpace(target), 1f));
                break;
            case RootWordEffect.EffectType.Prefab:
                if (effect.effectPrefab != null)
                {
                    var effectVisual = Instantiate(effect.effectPrefab);
                    effectVisual.transform.position = Battlefield.instance.GetSpace(target);
                    var fxComponent = effectVisual.GetComponent<SpellFx>();
                    if (fxComponent != null)
                        yield return fxComponent.StartCoroutine(fxComponent.PlayEffect());
                }
                break;
        }
    }
}
