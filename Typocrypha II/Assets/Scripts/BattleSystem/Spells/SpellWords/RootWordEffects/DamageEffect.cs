﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : RootWordEffect
{
    public Damage.FormulaType formula;
    public CustomFormula customFormula;
    public int power;

    public override PopupData Cast(Caster caster, Caster target)
    {
        if (formula == Damage.FormulaType.Custom)
            return customFormula.Apply(this, caster, target);
        return Damage.StandardFormula[formula](this, caster, target);
    }
}
