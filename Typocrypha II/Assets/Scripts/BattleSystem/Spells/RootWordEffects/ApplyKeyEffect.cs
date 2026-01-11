

public abstract class ApplyKeyEffect : RootWordEffect
{    
    public override CastResults Cast(Caster caster, Caster target, RootCastData spellData, Damage.DamageModifier mod, RootCastResults prevResults = null)
    {
        var results = InitializeCastResults(caster, target, mod);
        if (!target.IsPlayer)
        {
            return results;
        }
        if (mod.specialModifier == Damage.SpecialModifier.CritBlock)
        {
            results.Miss = true;
            return results;
        }
        Damage.StandardHitCheck(results, this, caster, target);
        if (results.Miss)
            return results;
        results.Effectiveness = Damage.GetReaction(this, caster, target, out float mult);
        if (Damage.ApplyReflect(results, this, caster, target, spellData))
            return results;
        ApplyKeyEffectFn(caster, target, spellData, mod.specialModifier, prevResults);
        return results;
    }

    protected abstract void ApplyKeyEffectFn(Caster caster, Caster target, RootCastData spellData, Damage.SpecialModifier mod, RootCastResults prevResults = null);
}
