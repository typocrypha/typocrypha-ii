using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Serializable dictionary wrapper (native Dictionary serialization not yet available)
// MUST BE INHERITED WITH TEMPLATE VARIABLES SET: generic classes are not serialized
public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver, IDictionary<TKey, TValue>, IEnumerable<KeyValuePair<TKey, TValue>>
{
    private Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>(); // Internal dictionary interface
    [SerializeField] private List<TKey> _keys = new List<TKey>(); // Serlializable list of keys; Keys and values match up 1-to-1
    [SerializeField] private List<TValue> _values = new List<TValue>(); // Serlializable list of values

    #region Dictionary Implementation
    public int Count
    {
        get
        {
            return _dictionary.Count;
        }
    }
    public void Clear()
    {
        _dictionary.Clear();
    }
    public bool ContainsKey(TKey key)
    {
        return _dictionary.ContainsKey(key);
    }
    public void Add(TKey key, TValue value)
    {
        _dictionary.Add(key, value);
    }
    public void Remove(TKey key)
    {
        _dictionary.Remove(key);
    }
    public TValue this[TKey key]
    {
        get
        {
            return _dictionary[key];
        }
        set
        {
            _dictionary[key] = value;
        }
    }
    public Dictionary<TKey,TValue>.KeyCollection Keys { get { return _dictionary.Keys; } }

    ICollection<TKey> IDictionary<TKey, TValue>.Keys => _dictionary.Keys;

    public ICollection<TValue> Values => _dictionary.Values;

    public bool IsReadOnly => false;
    #endregion

    #region IEnumarable Implementation
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return _dictionary.GetEnumerator();
    }

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
        return _dictionary.GetEnumerator();
    }
    #endregion

    #region Serialization
    public void OnBeforeSerialize()
    {
        _keys.Clear();
        _values.Clear();
        foreach (var kvp in _dictionary)
        {
            _keys.Add(kvp.Key);
            _values.Add(kvp.Value);
        }
    }

    // Convert lists back into dictionary
    public void OnAfterDeserialize()
    {
        _dictionary = new Dictionary<TKey, TValue>();
        for (int i = 0; i != System.Math.Min(_keys.Count, _values.Count); ++i)
        {
            try
            {
                _dictionary.Add(_keys[i], _values[i]);
            }
            catch(System.Exception e)
            {
                Debug.LogError("SDict Error: " + e.ToString() + " " + this.ToString());
            }
        }
    }

    bool IDictionary<TKey, TValue>.Remove(TKey key)
    {
        return _dictionary.Remove(key);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        return _dictionary.TryGetValue(key, out value);
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        _dictionary.Add(item.Key, item.Value);
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return _dictionary.ContainsKey(item.Key);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        throw new System.NotImplementedException("Cannot copy to serialized dict");
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return _dictionary.Remove(item.Key);
    }
    #endregion
}

[System.Serializable] public class SDictStringString : SerializableDictionary<string, string> { };
[System.Serializable] public class SDictStringInt : SerializableDictionary<string, int> { };
[System.Serializable] public class SDictStringFloat : SerializableDictionary<string, float> { };
