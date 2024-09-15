using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeOfTheStormEffect : RootWordEffect
{
    public override CastResults Cast(Caster caster, Caster target, RootCastData spellData, Damage.DamageModifier mod, RootCastResults prevResults = null)
    {
        if (!target.HasTag("StormOrb"))
        {
            return null;
        }
        var results = new CastResults(caster, target, target.Health)
        {
            Miss = false,
        };
        if (target.HasTag("FireBody"))
        {
            Typocrypha.Keyboard.instance.ApplyEffectRandom("KeyEffectBurning", caster, 3);
        }
        else if (target.HasTag("LightningBody"))
        {
            Typocrypha.Keyboard.instance.ApplyEffectRandom("KeyEffectShocked", caster);
        }
        else if (target.HasTag("IceBody"))
        {
            Typocrypha.Keyboard.instance.ApplyEffectRandom("KeyEffectFrozen", caster, 5);
        }
        target.Damage(target.Health);
        return results;
    }
}
