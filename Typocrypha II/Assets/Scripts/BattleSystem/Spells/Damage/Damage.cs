using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Damage
{
    public enum FormulaType
    {
        Standard,
        Custom,
    }
    public delegate PopupData Formula(DamageEffect effect, Caster caster, Caster target);

    public static Dictionary<FormulaType, Formula> StandardFormula { get; } = new Dictionary<FormulaType, Formula>
    {
        { FormulaType.Standard, Standard }
    };   
    private static PopupData Standard(DamageEffect effect, Caster caster, Caster target)
    {
        target.Health -= effect.power;
        return new PopupData
        {
            damage = effect.power,
        };
    }
}
