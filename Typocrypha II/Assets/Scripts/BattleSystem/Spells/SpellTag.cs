using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "SpellTag", menuName = "Tag/Spell Tag")]
public class SpellTag : ScriptableObject, IComparable<SpellTag>
{
    #region Name, Description, and Documentation Fields
    public string internalName = string.Empty;
    [Tooltip("Overrides how the tag is named in-game. uses the internal name if empty.")]
    [SerializeField]
    private string displayName = string.Empty;
    public string DisplayName { get => displayName != string.Empty ? displayName : internalName; }
    [TextArea(2,4)]
    [SerializeField]
    [Tooltip("Overrides how the tag is described in-game. uses the documentation if empty.")]
    private string description = string.Empty;
    public string DisplayDescription { get => description != string.Empty ? description : documentation; }
    [TextArea(2, 4)]
    public string documentation = string.Empty;
    #endregion

    public static SpellTag GetByName(string name)
    {
        if(SpellTagLookup.instance == null)
        {
            Debug.LogError("Trying to look up spell tag by name with no lookup instance. " +
                "Either there is none in the scene or you are trying to use this method from editor mode");
            return null;
        }
        return SpellTagLookup.instance.Get(name);
    }

    public int CompareTo(SpellTag other)
    {
        return internalName.CompareTo(other.internalName);
    }

    [System.Serializable]
    public class TagSet : SerializableSet<SpellTag>
    {
        public bool Contains(string tagName)
        {
            var tag = GetByName(tagName);
            if (tag == null)
                return false;
            return Contains(tag);
        }

        public void Add(string tagName)
        {
            var tag = GetByName(tagName);
            if (tag == null || Contains(tag))
                return;
            Add(tag);
        }

        public override string ToString()
        {
            string ret = string.Empty;
            foreach (var tag in this)
                ret += tag.internalName + ", ";
            return ret.TrimEnd().TrimEnd(',');
        }
    }
}
