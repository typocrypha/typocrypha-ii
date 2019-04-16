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

    /// <summary> Cast the spell's effect with a given caster at a given target position </summary>
    public void Cast (SpellWord[] words, Caster caster, Battlefield.Position target)
    {
        StartCoroutine(CastCR(Modify(words), caster, target));
    }
    /// <summary> Modify the root words by the modifiers and return the modified roots </summary>
    private RootWord[] Modify(SpellWord[] words)
    {
        SpellWord[] cloneWords = words.Select((word) => word.Clone()).ToArray();
        for(int i = 0; i < cloneWords.Length; ++i)
        {
            var mod = cloneWords[i] as ModifierWord;
            mod?.Modify(cloneWords, i);
        }
        return (cloneWords.Where((word) => word is RootWord).Select((word) => word as RootWord)).ToArray();
    }
    /// <summary> Cast the spell effects and play the associated fx</summary>
    private IEnumerator CastCR(RootWord[] roots, Caster caster, Battlefield.Position target)
    {
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
                        var fx = new SpellFxData[] { root.leftMod?.fx, effect.fx, root.rightMod?.fx };
                        crList.Add(SpellFxManager.instance.Play(fx, popupData, targetSpace, casterSpace));
                        yield return new WaitForSeconds(delayBetweenTargets);
                        //Break if enemy killed?
                    }                 
                }
                foreach (var cr in crList)
                    yield return cr;
            }
        }
    }

}
