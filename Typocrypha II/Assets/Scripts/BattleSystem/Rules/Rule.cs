using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Rule
{
    private static Rule activeRule = null;
    public static Rule ActiveRule
    {
        get => activeRule;
        set
        {
            if (activeRule != null)
            {
                activeRule.Remove();
            }
            activeRule = value;
            if(activeRule != null)
            {
                activeRule.Add();
            }
        }
    }

    public string displayName;
    public delegate bool ApplyRule(RootWordEffect effect, Caster caster, Caster target);
    public virtual bool ApplyToEffect(RootWordEffect effect, Caster caster, Caster target) => false;
    public virtual int CooldownModifier(SpellWord word) => 0;
    protected virtual void Add() { }
    protected virtual void Remove() { }
}
