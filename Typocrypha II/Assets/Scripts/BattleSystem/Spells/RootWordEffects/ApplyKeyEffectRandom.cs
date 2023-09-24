using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyKeyEffectRandom : ApplyKeyEffect
{
    public GameObject keyEffectPrefab;

    protected override void ApplyKeyEffectFn(Caster caster, Caster target, RootCastData spellData, Damage.SpecialModifier mod, RootCastResults prevResults = null)
    {
        Typocrypha.Keyboard.instance.ApplyEffectRandom(keyEffectPrefab, caster);
    }
}
