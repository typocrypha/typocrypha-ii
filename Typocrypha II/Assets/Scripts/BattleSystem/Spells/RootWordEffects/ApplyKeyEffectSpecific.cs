using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyKeyEffectSpecific : ApplyKeyEffect
{
    public GameObject keyEffectPrefab;
    public string affectedKeys;

    protected override void ApplyKeyEffectFn(Caster caster, Caster target, RootCastData spellData, Damage.SpecialModifier mod, RootCastResults prevResults = null)
    {
        Typocrypha.Keyboard.instance.ApplyEffect(affectedKeys, keyEffectPrefab, caster);
    }
}
