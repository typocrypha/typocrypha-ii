using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum ReactionType
{
    ANY = -100,
    WEAK = -1,
    NEUTRAL,
    RESIST,
    BLOCK,
    DODGE,
    DRAIN,
    REPEL,
}

[CreateAssetMenu(fileName = "CasterTag", menuName = "Tag/Caster Tag")]
public class CasterTag : ScriptableObject, System.IComparable<CasterTag>
{
    public string displayName = string.Empty;
    public CasterStats statMods;
    public ReactionDict reactions;
    public TagSet subTags;
    public AbilitySet abilities;

    public int CompareTo(CasterTag other)
    {
        return name.CompareTo(other.name);
    }

    [System.Serializable]
    public class Reaction
    {
        public SpellTag reactTo;
        public ReactionType reactionType = ReactionType.NEUTRAL;
    }
    [System.Serializable] public class ReactionDict : SerializableDictionary<string, Reaction> { }
    [System.Serializable] public class TagSet : SerializableSet<CasterTag> { }
    [System.Serializable] public class AbilitySet : SerializableSet<CasterAbility> { }
}

public class CasterAbility
{

}


