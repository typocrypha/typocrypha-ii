using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "SpellTag", menuName = "Tag/Spell Tag")]
public class SpellTag : ScriptableObject, IComparable<SpellTag>, IEquatable<SpellTag>
{
    public string displayName = string.Empty;
    [TextArea(2,4)]
    public string description = string.Empty;

    public int CompareTo(SpellTag other)
    {
        return name.CompareTo(other.name);
    }

    public override bool Equals(object other)
    {
        var tag = other as SpellTag;
        return tag == null ? false : displayName == tag.displayName;
    }

    public bool Equals(SpellTag other)
    {
        return other != null  &&
               displayName == other.displayName;
    }

    public override int GetHashCode()
    {
        var hashCode = -181192468;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(displayName);
        return hashCode;
    }

    public static bool operator ==(SpellTag tag1, SpellTag tag2)
    {
        return EqualityComparer<SpellTag>.Default.Equals(tag1, tag2);
    }

    public static bool operator !=(SpellTag tag1, SpellTag tag2)
    {
        return !(tag1 == tag2);
    }

    [System.Serializable]
    public class TagSet : SerializableSet<SpellTag>
    {
        public bool Contains(string tagName)
        {
            throw new System.NotImplementedException("add spell tag assetbundle");
            //return Contains(SpellTagIndex.getTagFromString(tagName));
        }
        public override string ToString()
        {
            string ret = string.Empty;
            foreach (var tag in this)
                ret += tag.name + ", ";
            return ret.TrimEnd().TrimEnd(',');
        }
    }
}
