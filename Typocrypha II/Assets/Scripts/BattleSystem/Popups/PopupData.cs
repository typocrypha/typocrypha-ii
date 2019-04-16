using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Effectiveness
{
    Weak,
    Neutral,
    Resist,
    Block,
    Dodge,
    Absorb,
    Repel,
}

public class PopupData
{
    public RootWordEffect effect;
    public List<SpellTag> tags;
    public Effectiveness effectiveness;
    public int damage;
    public GameObject popupPrefab = null;
}
