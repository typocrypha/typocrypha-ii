public class ClearKeyEffectsEffect : RootWordEffect
{
    public override bool CanCrit => false;
    public override CastResults Cast(Caster caster, Caster target, RootCastData spellData, Damage.DamageModifier mod, RootCastResults prevResults = null)
    {
        Typocrypha.Keyboard.instance.ClearKeyEffects();
        return new CastResults(caster, target) { DisplayDamage = false, };
    }
}
