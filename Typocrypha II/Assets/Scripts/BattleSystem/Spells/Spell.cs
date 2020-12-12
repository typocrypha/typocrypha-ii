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
    [SerializeField]
    private List<SpellWord> items = new List<SpellWord>();

    public float Cost { get => items.Select((item) => item.cost).Sum(); }

    public Sprite Icon => items.Where((w) => w is RootWord).Select((w) => w as RootWord).FirstOrDefault().icon;

    public bool Countered => items.Count == 1 && items[0].Key == SpellManager.instance.counterWord.Key;

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
        return items.Select((s) => s.internalName.ToUpper()).Aggregate((a, b) => a + "-" + b);
    }

    public IEnumerable<FieldObject> AllTargets(Battlefield.Position casterPos, Battlefield.Position targetPos)
    {
        var roots = SpellManager.instance.Modify(this);
        var targets = new HashSet<Battlefield.Position>();
        foreach(var root in roots)
        {
            foreach(var effect in root.effects)
            {
                var pattern = effect.pattern.Target(casterPos, targetPos);
                foreach(var space in pattern)
                    targets.Add(space);
            }
        }
        return targets.Select((s) => Battlefield.instance.GetObject(s));
    }

    #region IList implementation

    public SpellWord this[int index] { get => items[index]; set => items[index] = value; }

    public int Count => items.Count;

    public bool IsReadOnly => false;

    public void Add(SpellWord item) => items.Add(item);

    public void Clear() => items.Clear();

    public bool Contains(SpellWord item) => items.Contains(item);

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
