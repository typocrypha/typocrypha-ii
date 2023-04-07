public class ClearReinforcementsEffect : RootWordEffect
{
    public override bool CanCrit => false;
    public override CastResults Cast(Caster caster, Caster target, RootCastData spellData, Damage.SpecialModifier mod, RootCastResults prevResults = null)
    {
        BattleManager.instance.ClearReinforcements();
        return new CastResults(caster, target) { DisplayDamage = false, };
    }
}
