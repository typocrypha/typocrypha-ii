using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastResults
{
    public RootWordEffect effect;
    public List<SpellTag> tags = new List<SpellTag>();
    public Reaction effectiveness = Reaction.Neutral;
    public float damage = 1;
    public float staggerDamage = 0;
    public GameObject popupPrefab = null;
}
