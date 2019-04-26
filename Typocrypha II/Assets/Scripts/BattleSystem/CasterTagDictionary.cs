﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class CasterTagDictionary
{
    [SerializeField] private CasterTag.TagSet tags = new CasterTag.TagSet();
    [SerializeField] private TagMultiSet allTags = new TagMultiSet();

    #region Dictionary Functions
    public bool ContainsTag(string tagName)
    {
        throw new System.NotImplementedException("add caster tags assetbundle");
        //return allTags.Contains(CasterTagIndex.getTagFromString(tagName));
    }
    public void Add(CasterTag tag)
    {
        if (tags.Contains(tag))
        {
            Debug.LogWarning("Cannot Add Duplicate caster tag: " + tag.name);
            return;
        }
        tags.Add(tag);
        AddWithSubTags(tag);
    }
    private void AddWithSubTags(CasterTag tag)
    {
        allTags.Add(tag);
        statMod.AddInPlace(tag.statMods);
        AddReactions(tag);
        foreach (CasterTag t in tag.subTags)
            AddWithSubTags(t);
    }
    private void AddReactions(CasterTag tag)
    {
        foreach (var reaction in tag.reactions)
        {
            if (!reactions.ContainsKey(reaction.Key))
                reactions.Add(reaction.Key, new ReactionMultiSet());
            reactions[reaction.Key].Add(reaction.Value);
        }
    }
    public void Remove(CasterTag tag)
    {
        tags.Remove(tag);
        RemoveWithSubTags(tag);
    }
    private void RemoveWithSubTags(CasterTag tag)
    {
        allTags.Remove(tag);
        statMod.SubtractInPlace(tag.statMods);
        RemoveReactions(tag);
        foreach (CasterTag t in tag.subTags)
            RemoveWithSubTags(t);
    }
    private void RemoveReactions(CasterTag tag)
    {
        foreach (var reaction in tag.reactions)
        {
            reactions[reaction.Key].Remove(reaction.Value);
            if (reactions[reaction.Key].Count <= 0)
                reactions.Remove(reaction.Key);
        }
    }
    #endregion

    public IEnumerable<CasterTag> TopLevelTags => tags;

    #region Aggregate Tag Data
    public void RecalculateAggregate()
    {
        statMod = new CasterStats();
        reactions.Clear();
        foreach (var tag in allTags)
        {
            statMod.AddInPlace(tag.statMods);
            AddReactions(tag);
        }
    }
    public CasterStats statMod;
    public ReactionMultiSet GetReactions(SpellTag tag)
    {
        return reactions.ContainsKey(tag) ? reactions[tag] : null;
    }
    private ReactionDict reactions = new ReactionDict();
    //public List<CasterAbility> abilities;
    #endregion

    [System.Serializable] private class TagMultiSet : SerializableMultiSet<CasterTag> { }
    [System.Serializable] private class ReactionDict : SerializableDictionary<SpellTag, ReactionMultiSet>
    {

    }
    [System.Serializable] public class ReactionMultiSet : SerializableMultiSet<Reaction>
    {
        public void AddSet(ReactionMultiSet other)
        {
            foreach(var reaction in other)
            {
                Add(reaction, other.Freq(reaction));
            }
        }
    }
}
