using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpellList : IList<Spell>
{
    [SerializeField]
    private List<Spell> items = new List<Spell>();

    #region IList implementation

    public Spell this[int index] { get => items[index]; set => items[index] = value; }

    public int Count => items.Count;

    public bool IsReadOnly => false;

    public void Add(Spell item) => items.Add(item);

    public void Clear() => items.Clear();

    public bool Contains(Spell item) => items.Contains(item);

    public void CopyTo(Spell[] array, int arrayIndex) => items.CopyTo(array, arrayIndex);

    public IEnumerator<Spell> GetEnumerator() => items.GetEnumerator();

    public int IndexOf(Spell item) => items.IndexOf(item);

    public void Insert(int index, Spell item) => items.Insert(index, item);

    public bool Remove(Spell item) => items.Remove(item);

    public void RemoveAt(int index) => items.RemoveAt(index);

    IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();
    #endregion
}
