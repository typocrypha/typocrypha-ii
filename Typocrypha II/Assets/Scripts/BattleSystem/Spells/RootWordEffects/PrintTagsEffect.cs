using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintTagsEffect : RootWordEffect
{
    public override CastResults Cast(Caster caster, Caster target)
    {
        Debug.Log("Tags on this effect: " + tags);
        return null;
    }
}
