using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : RootWordEffect
{
    public Damage.FormulaType formula;
    [SubSO("Formula")]
    public CustomFormula customFormula;
    public int power;

    public override bool CanCrit => true;

    public override CastResults Cast(Caster caster, Caster target, bool crit, Spell spell, RootCastResults prevResults = null)
    {
        if (formula == Damage.FormulaType.Custom)
            return customFormula.Apply(this, caster, target, crit, spell, prevResults);
        return Damage.PresetFormulae[formula](this, caster, target, crit, spell);
    }
}
