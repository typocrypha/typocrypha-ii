using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagLookup : MonoBehaviour
{
    public static TagLookup instance = null;
    public SpellTagBundle spellTagBundle;
    public CasterTagBundle casterTagBundle;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public SpellTag GetSpellTag(string name)
    {
        if (spellTagBundle.tags.ContainsKey(name))
            return spellTagBundle.tags[name];
        return null;
    }

    public CasterTag GetCasterTag(string name)
    {
        if (casterTagBundle.tags.ContainsKey(name))
            return casterTagBundle.tags[name];
        return null;
    }
}
