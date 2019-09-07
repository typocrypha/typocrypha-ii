using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RootWordEffect : ScriptableObject
{
    public SpellFxData fx;
    public SpellTag.TagSet tags = new SpellTag.TagSet();
    public TargetData pattern = new TargetData();
    public abstract CastResults Cast(Caster caster, Caster target, RootCastResults prevResults = null);

    protected void LogMessage(string message, Sprite image = null, GameObject popupPrefabOverride = null)
    {
        SpellFxManager.instance.LogMessage(message, image, popupPrefabOverride);
    }
}
