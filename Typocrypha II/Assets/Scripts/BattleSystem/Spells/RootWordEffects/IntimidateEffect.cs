﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntimidateEffect : RootWordEffect
{
    public override CastResults Cast(Caster caster, Caster target, RootCastResults prevResults = null)
    {
        var results = new CastResults(caster, target);
        results.DisplayDamage = false;

        if(target.HasTag("Melancholy"))
        {
            target.AddTag("Crying");
            LogMessage(target.DisplayName + " is crying!");
        }
        else if(target.HasTag("Excitable"))
        {
            results.StaggerDamage = target.Stagger;
            target.Stagger = 0;            
            LogMessage(target.DisplayName + " is shocked!");
        }
        else if(target.HasTag("Bold"))
        {
            target.AddTag("Pissed");
            LogMessage(target.DisplayName + " is pissed off!");
        }
        else
        {
            LogMessage(target.DisplayName + " was unaffected!");
        }
        return results;
    }
}
