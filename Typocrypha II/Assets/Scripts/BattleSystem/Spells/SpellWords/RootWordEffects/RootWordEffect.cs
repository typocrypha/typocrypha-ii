using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RootWordEffect : ScriptableObject
{
    #region Visuals and Sfx
    public enum EffectType
    {
        Single,
        Sequence,
        Parallel,
        Prefab
    }
    public EffectType effectType = EffectType.Single;
    public GameObject effectPrefab = null;
    public List<SpellAnimationPacket> effectPackets = new List<SpellAnimationPacket>() { new SpellAnimationPacket() };
    #endregion

    public TargetData pattern = new TargetData();
    public abstract void Cast(Caster caster, Caster target);
}
