using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchData : MonoBehaviour
{
    [SerializeField] private List<string> researchKeys;
    [SerializeField] private List<SpellWord> researchWords;
    private readonly Dictionary<string, float> values = new Dictionary<string, float>();
    private readonly HashSet<string> decoded = new HashSet<string>();
    private readonly Dictionary<string, SpellWord> words = new Dictionary<string, SpellWord>();

    private void Awake()
    {
        for(int i = 0; i < Mathf.Min(researchKeys.Count, researchWords.Count); ++i)
        {
            if (!words.ContainsKey(researchKeys[i]))
            {
                words.Add(researchKeys[i], researchWords[i]);
            }
        }
    }

    public void Add(string key, float amount)
    {
        if (values.ContainsKey(key))
        {
            values[key] += amount;
        }
        else
        {
            values.Add(key, amount);
        }
    }

    public float Value(string key) => values.ContainsKey(key) ? values[key] : 0;

    public bool ReadyToDecode(string key) => Value(key) >= 1 && !IsDecoded(key);

    public bool IsDecoded(string key) => decoded.Contains(key);

    public void SetDecoded(string key)
    {
        if (IsDecoded(key))
            return;
        decoded.Add(key);
    }

    public SpellWord GetWord(string key)
    {
        return words.ContainsKey(key) ? words[key] : null;
    }
}
