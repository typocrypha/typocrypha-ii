using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SpellFxManager))]
public class SpellManager : MonoBehaviour
{
    private const float delayBetweenTargets = 0.1f;
    public static SpellManager instance;
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
        var roots = Modify(spell);
        var casterSpace = Battlefield.instance.GetSpace(caster.FieldPos);
        List<Coroutine> crList = new List<Coroutine>();
        foreach (var root in roots)
        {
            foreach (var effect in root.effects)
            {
                var targets = effect.pattern.Target(caster.FieldPos, target);
                crList.Clear();                
                foreach (var t in targets)
                {                   
                    var targetCaster = Battlefield.instance.GetCaster(t); 
                    var targetSpace = Battlefield.instance.GetSpace(t);
                    if (targetCaster == null)
                    {
                        crList.Add(SpellFxManager.instance.NoTargetFx(targetSpace));
                    }
                    else
                    {
                        var popupData = effect.Cast(caster, targetCaster);
                        // Update AI
                        targetCaster.GetComponent<CasterAI>()?.OnAfterHit?.Invoke(spell, effect, caster, popupData);
                        // Play Effects
                        var fx = new SpellFxData[] { root.leftMod?.fx, effect.fx, root.rightMod?.fx };
                        crList.Add(SpellFxManager.instance.Play(fx, popupData, targetSpace, casterSpace));
                        // Wait for delay between targets
                        yield return new WaitForSeconds(delayBetweenTargets);
                    }                 
                }
                // Wait for all of the animations to finish
                foreach (var cr in crList)
                    yield return cr;
            }
        }
    }
}
