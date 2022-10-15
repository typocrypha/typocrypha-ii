using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CustomFormula : ScriptableObject
{
    public abstract CastResults Apply(DamageEffect effect, Caster caster, Caster target, bool crit, RootCastData spellData, RootCastResults prevResults = null);

    protected void LogMessage(string message, Sprite image = null)
    {
        SpellFxManager.instance.LogMessage(message, image);
    }
}
