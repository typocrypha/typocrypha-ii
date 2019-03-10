﻿using System.Collections;
using System.Collections.Generic;
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
    
    private IEnumerator CastCR(RootWord[] roots, Caster caster, Battlefield.Position target)
    {
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
                    if (targetCaster == null)
                    {
                        var noTargetEffect = Instantiate(noTargetFxPrefab);
                        noTargetEffect.transform.position = Battlefield.instance.GetSpace(t);
                        var fxComponent = noTargetEffect.GetComponent<SpellFx>();
                        if (fxComponent != null)
                            crList.Add(fxComponent.StartCoroutine(fxComponent.PlayEffect()));
                    }
                    else
                    {
                        effect.Cast(caster, targetCaster); //TODO: add effect logging?
                        crList.Add(StartCoroutine(effect.fx.Play(Battlefield.instance.GetSpace(t))));
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
