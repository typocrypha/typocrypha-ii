using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "SpellTag", menuName = "Tag/Spell Tag")]
public class SpellTag : ScriptableObject, System.IComparable<SpellTag>
{
    public string displayName = string.Empty;

    public int CompareTo(SpellTag other)
    {
        return name.CompareTo(other.name);
    }

    public override bool Equals(object other)
    {
        var tag = other as SpellTag;
        return tag == null ? false : name == tag.name;  
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
