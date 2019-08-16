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
    #region Name, Description, and Documentation Fields
    public string internalName = string.Empty;
    public string displayName = string.Empty;
    public string DisplayName { get => displayName != string.Empty ? displayName : internalName; }
    public string description = string.Empty;
    public string DisplayDescription { get => description != string.Empty ? description : documentation; }
    public string documentation = string.Empty;
    #endregion

    public CasterStats statMods = new CasterStats();
    public ReactionDict reactions = new ReactionDict();
    public TagSet subTags = new TagSet();
    [SubSO("Ability1")]
    public CasterAbility ability1;
    [SubSO("Ability2")]
    public CasterAbility ability2;
    public int CompareTo(CasterTag other)
    {
        return internalName.CompareTo(other.internalName);
    }

    [System.Serializable] public class ReactionDict : SerializableDictionary<SpellTag, Reaction> { }
    [System.Serializable] public class TagSet : SerializableSet<CasterTag> { }
}
