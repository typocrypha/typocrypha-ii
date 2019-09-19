using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Caster))]
public class ApplyStatusOnAwake : MonoBehaviour
{
    public StatusEffect statusEffectPrefab;
    // Start is called before the first frame update
    void Start()
    {
        var caster = GetComponent<Caster>();
        caster.AddTagWithStatusEffect(statusEffectPrefab, statusEffectPrefab.casterTag);
    }
}
