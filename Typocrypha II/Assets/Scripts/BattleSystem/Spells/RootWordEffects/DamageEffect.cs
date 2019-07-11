using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : RootWordEffect
{
    public Damage.FormulaType formula;
    [SubSO("Formula")]
    public CustomFormula customFormula;
    public int power;

    public override CastResults Cast(Caster caster, Caster target)
    {
        if (formula == Damage.FormulaType.Custom)
            return customFormula.Apply(this, caster, target);
        return Damage.PresetFormula[formula](this, caster, target);
    }
}
