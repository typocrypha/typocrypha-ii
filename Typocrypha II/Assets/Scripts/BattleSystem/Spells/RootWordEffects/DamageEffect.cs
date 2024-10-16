﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : RootWordEffect
{
    public Damage.FormulaType formula;
    [SubSO("Formula")]
    public CustomFormula customFormula;
    public int power;

    public override CastResults Cast(Caster caster, Caster target, RootCastResults prevResults = null)
    {
        if (formula == Damage.FormulaType.Custom)
            return customFormula.Apply(this, caster, target, prevResults);
        return Damage.PresetFormulae[formula](this, caster, target);
    }
}
