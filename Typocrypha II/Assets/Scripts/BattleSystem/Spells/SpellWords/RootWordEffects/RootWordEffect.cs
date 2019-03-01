using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RootWordEffect : ScriptableObject
{
    public GameObject effect = null;
    public TargetData pattern = new TargetData();
    public abstract void Cast(Caster caster, Caster target);
}
