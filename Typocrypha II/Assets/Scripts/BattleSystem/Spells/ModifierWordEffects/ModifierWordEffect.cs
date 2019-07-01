using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModifierWordEffect : ScriptableObject
{
    public virtual bool OverrideTags { get; protected set; }
    public abstract void ApplyEffect(RootWordEffect effect, SpellTag.TagSet tags);
}
