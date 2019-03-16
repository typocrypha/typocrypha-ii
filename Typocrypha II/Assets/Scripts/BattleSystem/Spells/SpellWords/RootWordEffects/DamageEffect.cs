using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : RootWordEffect
{
    public Damage.FormulaType formula;
    public CustomFormula customFormula;
    public int power;

    public override void Cast(Caster caster, Caster target)
    {
        if (formula == Damage.FormulaType.Custom)
            customFormula.Apply(this, caster, target);
        else
            Damage.StandardFormula[formula](this, caster, target);
    }
}
