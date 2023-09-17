using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellLookup : MonoBehaviour
{
    public static SpellLookup instance = null;
    [SerializeField] private SpellTagBundle spellTagBundle;
    [SerializeField] private CasterTagBundle casterTagBundle;
    [SerializeField] private SpellWordBundle allWordsBundle;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    /// <summary>
    /// If the string is a valid spell tag, returns it (else returns null)
    /// </summary>
    public SpellTag GetSpellTag(string name)
    {
        if (spellTagBundle.tags.ContainsKey(name))
            return spellTagBundle.tags[name];
        return null;
    }
    /// <summary>
    /// If the string is a valid caster tag, returns it (else returns null)
    /// </summary>
    public CasterTag GetCasterTag(string name)
    {
        if (casterTagBundle.tags.ContainsKey(name))
            return casterTagBundle.tags[name];
        return null;
    }

    public SpellWord GetSpellWord(string name)
    {
        return allWordsBundle.words.TryGetValue(name.ToLower(), out var word) ? word : null;
    }
}
