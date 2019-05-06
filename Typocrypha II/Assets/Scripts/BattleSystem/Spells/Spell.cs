using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
/// <summary>
/// Basically just a list of spellwords.
/// Implemented this way instead of inheriting from List due the the way unity property drawers work
/// </summary>
public class Spell : IEnumerable<SpellWord>, IList<SpellWord>
{
    [SerializeField]
    private List<SpellWord> items = new List<SpellWord>();

    public string ToDisplayString()
    {
        return items.Select((s) => s.displayName.ToUpper()).Aggregate((a, b) => a + "-" + b);
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
}
