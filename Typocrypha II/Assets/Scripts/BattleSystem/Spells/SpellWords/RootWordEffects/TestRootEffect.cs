using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRootEffect : RootWordEffect
{
    public string debugMessage = string.Empty;
    public override void Cast(Caster caster, Caster target)
    {
        Debug.Log(debugMessage);
    }
}
