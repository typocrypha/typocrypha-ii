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
        var se = Instantiate(statusEffectPrefab, caster.transform).GetComponent<StatusEffect>();
        caster.AddTagWithStatusEffect(se, se.casterTag);
    }
}
