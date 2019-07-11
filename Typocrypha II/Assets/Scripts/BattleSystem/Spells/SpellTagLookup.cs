using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellTagLookup : MonoBehaviour
{
    public static SpellTagLookup instance = null;
    public SpellTagBundle bundle;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public SpellTag Get(string name)
    {
        if (bundle.tags.ContainsKey(name))
            return bundle.tags[name];
        return null;
    }
}
