using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SpellFxManager))]
[RequireComponent(typeof(SpellRestrictions))]
public class SpellManager : MonoBehaviour
{
    private const float delayBetweenTargets = 0.1f;
    private const float delayBeforeLog = 0.25f;
    public static SpellManager instance;
    public SpellWord counterWord;

    /// <summary> Singleton implementation </summary>
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary> 
    /// Cast the spell's effect with a given caster at a given target position 
    /// Returns the case coroutine (in case the end of casting must be waited on)
    /// </summary>
    public Coroutine Cast (Spell spell, Caster caster, Battlefield.Position target)
    {
        return StartCoroutine(CastCR(spell, caster, target));
    }

    /// <summary> 
    /// Cast the spell's effect with a given caster at a given target position, and then Cancels 
    /// Returns the case coroutine (in case the end of casting must be waited on)
    /// </summary>
    public Coroutine CastAndCounter(Spell spell, Caster caster, Battlefield.Position target)
    {
        var targetCaster = Battlefield.instance.GetCaster(target);
        if (targetCaster == null)
            return StartCoroutine(CastCR(spell, caster, target));
        return StartCoroutine(CastAndCounterCR(spell, caster, target, (c) => c == targetCaster));
    }
    /// <summary> Modify the root words by the modifiers and return the modified roots </summary>
    public RootWord[] Modify(Spell spell)
    {
        SpellWord[] cloneWords = spell.Select((word) => word.Clone()).ToArray();
        for(int i = 0; i < cloneWords.Length; ++i)
        {
            var mod = cloneWords[i] as ModifierWord;
            mod?.Modify(cloneWords, i);
        }
        return (cloneWords.Where((word) => word is RootWord).Select((word) => word as RootWord)).ToArray();
    }
    /// <summary> Cast the spell effects and play the associated fx</summary>
    private IEnumerator CastCR(Spell spell, Caster caster, Battlefield.Position target)
    {
        // If the spell is restricted, break and do not cast
        if (SpellRestrictions.instance.IsRestricted(spell, caster, target, true))
        {
            if (SpellFxManager.instance.HasMessages)
            {
                yield return new WaitForSeconds(delayBeforeLog);
                yield return SpellFxManager.instance.PlayMessages();
            }
            yield break;
        }
        SpellFxManager.instance.LogMessage(caster.DisplayName + " casts " + spell.ToDisplayString(), spell.Icon);
        yield return SpellFxManager.instance.PlayMessages();
        var roots = Modify(spell);
        var casterSpace = Battlefield.instance.GetSpace(caster.FieldPos);
        List<Coroutine> crList = new List<Coroutine>();
        foreach (var root in roots)
        {
            // Log the effect of each effect
            var rootResults = new RootCastResults();
            foreach (var effect in root.effects)
            {
                // Get the effect's targets
                var targets = effect.pattern.Target(caster.FieldPos, target);
                // Log the effect of each effect
                var effectResults = new List<CastResults>();
                crList.Clear();                
                foreach (var t in targets)
                {                   
                    var targetCaster = Battlefield.instance.GetCaster(t); 
                    var targetSpace = Battlefield.instance.GetSpace(t);
                    if (targetCaster == null || targetCaster.BStatus == Caster.BattleStatus.Dead || targetCaster.BStatus == Caster.BattleStatus.Fled)
                    {
                        caster.OnNoTargetHit?.Invoke(t);
                        crList.Add(SpellFxManager.instance.NoTargetFx(targetSpace));
                    }
                    else
                    {
                        // Apply the rule effect if necessary
                        Rule.ActiveRule?.Apply?.Invoke(effect, caster, targetCaster);
                        // Apply OnCast Callbacks
                        caster.OnBeforeSpellEffectResolved?.Invoke(effect, caster, targetCaster);
                        // Cast the effect
                        var castResults = effect.Cast(caster, targetCaster, rootResults);
                        // Apply OnHit Callbacks (Updates AI)
                        targetCaster.OnAfterHitResolved?.Invoke(effect, caster, targetCaster, castResults);
                        // Play Effects
                        var fx = new SpellFxData[] { root.leftMod?.fx, effect.fx, root.rightMod?.fx };
                        crList.Add(SpellFxManager.instance.Play(fx, castResults, targetSpace, casterSpace));
                        // Log the results of this target
                        effectResults.Add(castResults);
                        // Wait for delay between targets
                        yield return new WaitForSeconds(delayBetweenTargets);
                    }                 
                }
                // Wait for all of the animations to finish
                foreach (var cr in crList)
                    yield return cr;
                // Apply callbacks after the effect is finished
                caster.OnAfterSpellEffectResolved?.Invoke(spell, caster);
                if (SpellFxManager.instance.HasMessages)
                {
                    yield return new WaitForSeconds(delayBeforeLog);
                    yield return SpellFxManager.instance.PlayMessages();
                }
                // Log the effects of this effect
                rootResults.Add(effectResults);
            }
        }
        // Apply callbacks after the whole cast is finished
        caster.OnAfterCastResolved?.Invoke(spell, caster);
    }

    private IEnumerator CastAndCounterCR(Spell spell, Caster caster, Battlefield.Position target, Predicate<Caster> pred)
    {
        yield return StartCoroutine(CastCR(spell, caster, target));
        var cancelTargets = Battlefield.instance.Casters.Where((c) => pred(c));
        foreach (var cancelTarget in cancelTargets)
        {
            var remainingWords = cancelTarget.Spell.Where((word) => !spell.Contains(word));
            // No words were countered, continue to next target
            if (remainingWords.Count() == cancelTarget.Spell.Count)
                continue;
            // Full counter (no remaining roots)
            if(remainingWords.Count((w) => w is RootWord) <= 0)
            {
                cancelTarget.Stagger--;
                cancelTarget.Spell = new Spell(counterWord);
            }
            else // Partial counter
            {
                cancelTarget.Spell = new Spell(remainingWords);
            }
            cancelTarget.OnCounter?.Invoke(cancelTarget);
            SpellFxManager.instance.LogMessage(cancelTarget.DisplayName + " has been countered!");
        }
        yield return SpellFxManager.instance.PlayMessages();
    }
}

/// <summary>
/// A class to store the results of the effects of an entire root
/// </summary>
public class RootCastResults : List<List<CastResults>>
{
    public List<CastResults> LastEffect => this[Count - 1];
}
