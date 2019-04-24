﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Reaction
{
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

    [System.Serializable] public class ReactionDict : SerializableDictionary<SpellTag, Reaction> { }
    [System.Serializable] public class TagSet : SerializableSet<CasterTag> { }
    [System.Serializable] public class AbilitySet : SerializableSet<CasterAbility> { }
}

[System.Serializable]
public class CasterAbility
{
    public enum AbilityName
    { }
    public SpellTag.TagSet spellTags;
    public CasterTag.TagSet casterTags;
    public List<SpellWord> spells;
    public List<TargetData> targetData;
    public SDictStringInt intParams;
    public SDictStringFloat floatParams;
    public Component component;

}
