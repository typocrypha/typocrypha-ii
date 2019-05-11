﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rule : MonoBehaviour
{
    private static Rule activeRule = null;
    public static Rule ActiveRule
    {
        get => activeRule;
        set
        {
            var res = SpellRestrictions.instance?.restrictions;
            if (res != null)
                foreach( var restriction in activeRule.Restrictions.GetInvocationList())
                    res -= restriction as SpellRestrictions.Restriction;
            activeRule = value;
            if (res != null)
                foreach (var restriction in activeRule.Restrictions.GetInvocationList())
                    res += restriction as SpellRestrictions.Restriction;
            activeRule = value;
        }
    }

    public string displayName;
    public delegate bool ApplyRule(RootWordEffect effect, Caster caster, Caster target);
    public ApplyRule Apply { get; set; }
    public SpellRestrictions.Restriction Restrictions { get; set; }
}
