using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[System.Serializable]
/// <summary>
/// Basically just a list of spellwords.
/// Implemented this way instead of inheriting from List due the the way unity property drawers work
/// </summary>
public class Spell : IList<SpellWord>, IEquatable<Spell>
{
    public const char separator = '-';
    public const char separatorModRight = '>';
    public const char separatorModLeft = '<';
    public static readonly char[] separators = { separator, separatorModLeft, separatorModRight };
    [SerializeField]
    private List<SpellWord> items = new List<SpellWord>();

    public float Cost
    {
        get
        {
            float cost = 0;
            foreach (var item in items)
            {
                cost += item.cost;
            }
            return cost;
        }
    }

    public Sprite Icon 
    {
        get
        {
            foreach(var item in items)
            {
                if(item is RootWord root)
                {
                    return root.icon;
                }
            }
            return null;
        }
    }

    public bool Countered => items.Count == 1 && SpellWord.CompareKeys(items[0], SpellManager.instance.counterWord);

    public int RootCount
    {
        get
        {
            int count = 0;
            foreach (var item in items)
            {
                if (item is RootWord)
                {
                    ++count;
                }
            }
            return count;
        }
    }

    public List<RootWord> Roots
    {
        get
        {
            var list = new List<RootWord>(Count);
            foreach (var item in items)
            {
                if (item is RootWord root)
                {
                    list.Add(root);
                }
            }
            return list;
        }
    }

    public Spell()
    {

    }

    public Spell(SpellWord word)
    {
        items.Add(word);
    }

    public Spell(IEnumerable<SpellWord> words)
    {
        items.AddRange(words);
    }

    public string ToDisplayString()
    {
        if (items.Count == 0)
            return string.Empty;
        string ret = string.Empty;
        for (int i = 0; i < Count; ++i)
        {
            var word = this[i];
            ret += word.internalName.ToUpper();
            if (i >= Count - 1)
            {
                break;
            }
            if (word is ModifierWord)
            {
                var modifier = word as ModifierWord;
                if ((modifier.direction == ModifierWord.Direction.Right || modifier.direction == ModifierWord.Direction.Bidirectional) && this[i + 1] is RootWord)
                {
                    ret += separatorModRight;
                }
                else
                {
                    ret += separator;
                }
            }
            else if(word is RootWord)
            {
                if (this[i + 1] is ModifierWord modifier && (modifier.direction == ModifierWord.Direction.Left || modifier.direction == ModifierWord.Direction.Bidirectional))
                {
                    ret += separatorModLeft;
                }
                else
                {
                    ret += separator;
                }
            }
        }
        return ret;
    }

    public IEnumerable<Caster> AllTargets(Battlefield.Position casterPos, Battlefield.Position targetPos)
    {
        var roots = SpellManager.instance.Modify(this);
        var targets = new HashSet<Battlefield.Position>();
        foreach(var root in roots)
        {
            foreach(var effect in root.effects)
            {
                var pattern = effect.pattern.Target(casterPos, targetPos);
                foreach(var space in pattern)
                {
                    if (!targets.Contains(space))
                    {
                        targets.Add(space);
                    }
                }
            }
        }
        return targets.Select(Battlefield.instance.GetCaster);
    }

    #region IList implementation

    public SpellWord this[int index] { get => items[index]; set => items[index] = value; }

    public int Count => items.Count;

    public bool IsReadOnly => false;

    public void Add(SpellWord item) => items.Add(item);

    public void Clear() => items.Clear();

    public bool Contains(SpellWord item) => items.Exists(word => word == item);

    public void CopyTo(SpellWord[] array, int arrayIndex) => items.CopyTo(array, arrayIndex);

    public IEnumerator<SpellWord> GetEnumerator() => items.GetEnumerator();

    public int IndexOf(SpellWord item) => items.IndexOf(item);

    public void Insert(int index, SpellWord item) => items.Insert(index, item);

    public bool Remove(SpellWord item) => items.Remove(item);

    public void RemoveAt(int index) => items.RemoveAt(index);

    IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();

    #endregion

    public bool Equals(Spell other)
    {
        return ToDisplayString().Equals(other.ToDisplayString());
    }
}
