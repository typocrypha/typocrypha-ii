using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastResults
{
    public Caster caster;
    public Caster target;
    public RootWordEffect effect;
    public GameObject popupPrefab = null;

    public bool DisplayDamage { get; set; } = true;
    public bool Miss { get; set; } = false;
    public bool Crit { get; set; } = false;
    public float StaggerDamage { get; set; } = 0;
    public float Damage { get; set; } = 1;
    public float Combo { get; set; } = 0;
    public Reaction Effectiveness { get; set; } = Reaction.Neutral;
    public float EffectivenessMagnitude { get; set; } = 1;

    public CastResults(Caster caster, Caster target)
    {
        this.caster = caster;
        this.target = target;
    }

    public bool WillDealDamage => Damage > 0 && (Effectiveness == Reaction.Neutral || Effectiveness == Reaction.Weak || Effectiveness == Reaction.Resist);
}
