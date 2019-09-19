using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusMarked : StatusEffect
{
    public bool stacks;
    public int intensity;

    public override void Apply(ApplyStatusEffect effect, Caster caster, Caster target, CastResults data)
    {
        if (stacks && target.HasTag(casterTag))
        {
            var mark = target.GetStatusEffect(casterTag) as StatusMarked;
            if (mark == null)
                return;
            mark.intensity += intensity;
        }

    }
}
