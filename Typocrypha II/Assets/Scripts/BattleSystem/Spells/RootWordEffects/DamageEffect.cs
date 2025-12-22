using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : RootWordEffect
{
    public override int Power => power;

    protected override bool CanCritInternal => true;

    public Damage.FormulaType formula;
    [SubSO("Formula")]
    public CustomFormula customFormula;
    public int power;

    public override CastResults Cast(Caster caster, Caster target, RootCastData spellData, Damage.DamageModifier mod, RootCastResults prevResults = null)
    {
        return formula == Damage.FormulaType.Custom ? customFormula.Apply(this, caster, target, mod, spellData, prevResults)
            : Damage.PresetFormulae[formula](this, caster, target, mod, spellData);
    }
}
