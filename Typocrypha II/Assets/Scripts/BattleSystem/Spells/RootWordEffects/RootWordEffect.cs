using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RootWordEffect : ScriptableObject
{
    public SpellFxData fx;
    public WordFxDefinition wordFx;
    public SpellTag.TagSet tags = new SpellTag.TagSet();
    public TargetData pattern = new TargetData();

    public virtual bool CanCrit => !tags.Contains("NeverCrit");
    public virtual int Power => 0;
    public abstract CastResults Cast(Caster caster, Caster target, RootCastData spellData, Damage.DamageModifier mod, RootCastResults prevResults = null);

    public virtual CastResults InitializeCastResults(Caster caster, Caster target, Damage.DamageModifier mod, int startingDamage = 0)
    {
        var results = new CastResults(caster, target, startingDamage);
        results.DisplayDamage = startingDamage != 0;
        results.AnimationData.Add(fx);
        results.WordFx = wordFx;
        results.Mod = mod.specialModifier;
        return results;
    }

    protected void LogMessage(string message, Sprite icon = null)
    {
        SpellFxManager.instance.LogMessage(message, icon);
    }
}
