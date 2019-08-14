using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class CasterTagDictionary
{
    [SerializeField] private CasterTag.TagSet tags = new CasterTag.TagSet();
    [SerializeField] private TagMultiSet allTags = new TagMultiSet();

    #region Dictionary Functions
    public int Count
    {
        get
        {
            return tags.Count;
        }
    }
    public bool ContainsTag(string tagName)
    {
        return allTags.Contains(TagLookup.instance.GetCasterTag(tagName));
    }
    public bool ContainsTag(CasterTag tag)
    {
        return allTags.Contains(tag);
    }
    public void Add(CasterTag tag, Caster addTo = null)
    {
        if (tags.Contains(tag))
        {
            Debug.LogWarning("Cannot Add Duplicate caster tag: " + tag.name);
            return;
        }
        tags.Add(tag);
        AddWithSubTags(tag, addTo);
    }
    private void AddWithSubTags(CasterTag tag, Caster addTo = null)
    {
        allTags.Add(tag);
        statMod.AddInPlace(tag.statMods);
        AddReactions(tag);
        if (addTo != null && tag.statusEffectPrefab != null)
            statusEffects.Add(tag, GameObject.Instantiate(tag.statusEffectPrefab, addTo.transform));
        foreach (CasterTag t in tag.subTags)
            AddWithSubTags(t, addTo);
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
        if(statusEffects.ContainsKey(tag))
        {
            GameObject.Destroy(statusEffects[tag]);
            statusEffects.Remove(tag);
        }
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
    public void SpawnAllStatusEffects(Caster addTo)
    {
        foreach(var tag in allTags)
            if (tag.statusEffectPrefab != null)
                statusEffects.Add(tag, GameObject.Instantiate(tag.statusEffectPrefab, addTo.transform));
    }
    private StatusEffectDict statusEffects = new StatusEffectDict();
    #endregion

    [System.Serializable] private class TagMultiSet : SerializableMultiSet<CasterTag> { }
    [System.Serializable] private class ReactionDict : SerializableDictionary<SpellTag, ReactionMultiSet>
    {

    }
    [System.Serializable] private class StatusEffectDict : SerializableDictionary<CasterTag, GameObject> { }
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
