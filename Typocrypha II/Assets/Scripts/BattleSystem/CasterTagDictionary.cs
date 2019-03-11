using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CasterTagDictionary
{
    [SerializeField] private CasterTag.TagSet tags = new CasterTag.TagSet();
    [SerializeField] private TagMultiSet allTags = new TagMultiSet();

    #region Dictionary Functions
    public bool ContainsTag(string tagName)
    {
        return allTags.Contains(CasterTagIndex.getTagFromString(tagName));
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
        foreach (CasterTag t in tag.subTags)
            AddWithSubTags(t);
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
        foreach (CasterTag t in tag.subTags)
            RemoveWithSubTags(t);
    }
    #endregion

    public IEnumerable<CasterTag> TopLevelTags => tags;

    #region Aggregate Tag Data
    public void RecalculateStats()
    {
        statMod = new CasterStats();
        foreach (var tag in allTags)
            statMod.AddInPlace(tag.statMods);
    }
    public CasterStats statMod;
    //public List<CasterAbility> abilities;
    #endregion

    [System.Serializable] private class TagMultiSet : SerializableMultiSet<CasterTag> { }
}
