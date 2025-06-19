using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RootWordEffect : ScriptableObject
{
    public SpellFxData fx;
    public SpellTag.TagSet tags = new SpellTag.TagSet();
    public TargetData pattern = new TargetData();

    public virtual bool CanCrit => !tags.Contains("NeverCrit");
    public abstract CastResults Cast(Caster caster, Caster target, RootCastData spellData, Damage.DamageModifier mod, RootCastResults prevResults = null);

    protected virtual CastResults InitializeCastResults(Caster caster, Caster target, Damage.DamageModifier mod, int startingDamage = 0)
    {
        var results = new CastResults(caster, target, startingDamage);
        results.DisplayDamage = startingDamage != 0;
        results.AnimationData.Add(fx);
        results.Mod = mod.specialModifier;
        return results;
    }

    protected void LogMessage(string message, Sprite icon = null)
    {
        SpellFxManager.instance.LogMessage(message, icon);
    }
}
