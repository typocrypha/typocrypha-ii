using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CasterAbility : ScriptableObject
{
    public abstract void AddTo(Caster caster);
    public abstract void RemoveFrom(Caster caster);
}
