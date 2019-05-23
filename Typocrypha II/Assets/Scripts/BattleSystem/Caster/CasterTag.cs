using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Reaction
{
    Weak = -1,
    Neutral,
    Resist,
    Block,
    Dodge,
    Drain,
    Repel,
}

[CreateAssetMenu(fileName = "CasterTag", menuName = "Tag/Caster Tag")]
public class CasterTag : ScriptableObject, System.IComparable<CasterTag>
{
    public string displayName = string.Empty;
    public CasterStats statMods;
    public ReactionDict reactions;
    public TagSet subTags;
    [SubSO("Ability1")]
    public CasterAbility ability1;
    [SubSO("Ability2")]
    public CasterAbility ability2;

    public int CompareTo(CasterTag other)
    {
        return displayName.CompareTo(other.displayName);
    }

    [System.Serializable] public class ReactionDict : SerializableDictionary<SpellTag, Reaction> { }
    [System.Serializable] public class TagSet : SerializableSet<CasterTag> { }
}
