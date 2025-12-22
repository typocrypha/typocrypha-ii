public class ClearReinforcementsEffect : RootWordEffect
{
    public override CastResults Cast(Caster caster, Caster target, RootCastData spellData, Damage.DamageModifier mod, RootCastResults prevResults = null)
    {
        BattleManager.instance.ClearReinforcements();
        return InitializeCastResults(caster, target, mod);
    }
}
