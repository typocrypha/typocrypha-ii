using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RootWordEffect : ScriptableObject
{
    public SpellFxData fx;
    public SpellTag.TagSet tags = new SpellTag.TagSet();
    public TargetData pattern = new TargetData();

    public abstract bool CanCrit { get; }
    public abstract CastResults Cast(Caster caster, Caster target, bool crit, Spell spell, RootCastResults prevResults = null);

    protected void LogMessage(string message, Sprite icon = null)
    {
        SpellFxManager.instance.LogMessage(message, icon);
    }
}
