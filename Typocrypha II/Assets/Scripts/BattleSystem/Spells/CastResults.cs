using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastResults
{
    public RootWordEffect effect;
    public GameObject popupPrefab = null;
    public float StaggerDamage { get; set; } = 0;
    public float Damage { get; set; } = 1;
    public Reaction Effectiveness { get; set; } = Reaction.Neutral;
}
