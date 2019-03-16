using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class RandomC
{
    private static System.Random _rand = new System.Random();

    public static int RandomInt(int min, int max) => _rand.Next(min, max);
    public static double RandomDouble() => _rand.NextDouble();

    // Returns an unweighted random choice from the given array
    public static T RandomChoice<T>(T[] items)
    {
        return items[_rand.Next(0, items.Length)];
    }
    public static T RandomChoice<T>(T[] items, int[] weights, bool isAbsolute)
    {
        // totalWeight is the sum of all weights, or 1 if absolute
        int totalWeight = isAbsolute ? 100 : weights.Aggregate((a, b) => a + b);
#if DEBUG
        if (isAbsolute && weights.Aggregate((a, b) => a + b) != 100)
            throw new System.Exception("Absolute weights do not add up to 100%! Items: " + items.ToString() + " Weights: " + weights.ToString());
#endif
        int randomNumber = _rand.Next(totalWeight);
        for (int i = 0; i < items.Length; ++i)
        {
            if (randomNumber < weights[i])
                return items[i];

            randomNumber -= weights[i];
        }
        throw new System.Exception("No item chosen");
    }
    public static T RandomChoice<T>(WeightedSet<T> items, bool isAbsolute = false)
    {
        return RandomChoice<T>(items.Items, items.Weights, isAbsolute);
    }
}

public class WeightedSet<T> : IEnumerable<KeyValuePair<T, int>>
{
    private Dictionary<T, int> _items = new Dictionary<T, int>();
    public T[] Items { get => _items.Keys.ToArray(); }
    public int[] Weights { get => _items.Values.ToArray(); }
    public int Count { get => _items.Count; }

    public WeightedSet() { }
    public WeightedSet(IEnumerable<T> items, IEnumerable<int> weights)
    {
        IEnumerator<int> e = weights.GetEnumerator();
        foreach (T item in items)
        {
            Add(item, e.Current);
            e.MoveNext();
        }
    }
    public WeightedSet(IEnumerable<KeyValuePair<T, int>> pairs)
    {
        foreach (var kvp in pairs)
            Add(kvp.Key, kvp.Value);
    }
    public void Add(T item, int weight = 1)
    {
        if (_items.ContainsKey(item))
            _items[item] += weight;
        else
            _items.Add(item, weight);
    }
    public override string ToString()
    {
        return _items.ToString();
    }

    #region IEnumerable Implementation
    public IEnumerator<KeyValuePair<T, int>> GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _items.GetEnumerator();
    }
    #endregion
}
