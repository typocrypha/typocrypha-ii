using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CustomFormula : ScriptableObject
{
    public abstract PopupData Apply(DamageEffect effect, Caster caster, Caster target);
}
