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
        addWithSubTags(tag);
    }
    private void addWithSubTags(CasterTag tag)
    {
        allTags.Add(tag);
        foreach (CasterTag t in tag.subTags)
            addWithSubTags(t);
    }
    public void Remove(CasterTag tag)
    {
        tags.Remove(tag);
        removeWithSubTags(tag);
    }
    private void removeWithSubTags(CasterTag tag)
    {
        allTags.Remove(tag);
        foreach (CasterTag t in tag.subTags)
            removeWithSubTags(t);
    }
    #endregion

    public IEnumerable<CasterTag> TopLevelTags => tags;

    #region Aggregate Tag Data  (TODO)
    public CasterStats statMod;
    public List<CasterAbility> abilities;
    #endregion

    [System.Serializable] private class TagMultiSet : SerializableMultiSet<CasterTag> { }
}
