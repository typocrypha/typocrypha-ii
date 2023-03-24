using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastResults
{
    public Caster caster;
    public Caster target;
    public RootWordEffect effect;

    public bool DisplayDamage { get; set; } = true;
    public bool Miss { get; set; } = false;
    public bool IsCrit => Mod == global::Damage.SpecialModifier.Critical;
    public Damage.SpecialModifier Mod { get; set; }
    public float StaggerDamage { get; set; } = 0;
    public bool Stun { get; set; } = false;
    public float Damage { get; set; } = 0;
    public float Combo { get; set; } = 0;
    public Reaction Effectiveness { get; set; } = Reaction.Neutral;
    public float EffectivenessMagnitude { get; set; } = 1;

    public CastResults(Caster caster, Caster target, int startingDamage = 0)
    {
        this.caster = caster;
        this.target = target;
        Damage = startingDamage;
    }

    public bool WillDealDamage => Damage > 0 && !Miss && (Effectiveness == Reaction.Neutral || Effectiveness == Reaction.Weak || Effectiveness == Reaction.Resist);
}
