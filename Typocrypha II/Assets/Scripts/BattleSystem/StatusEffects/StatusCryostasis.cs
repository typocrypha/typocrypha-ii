using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusCryostasis : StatusRemoveAfterHitOrCast
{
    private string affectedKeys = string.Empty;
    public override void Apply(ApplyStatusEffect effect, Caster caster, Caster target, CastResults data)
    {
        if(target.IsPlayer)
            affectedKeys = Typocrypha.Keyboard.instance.ApplyEffectRandom("KeyEffectFrozen", 20);
    }

    public override void OnBeforeEffectApplied(RootWordEffect effect, Caster caster, Caster target, RootCastData spellData, CastResults data)
    {
        if(!effect.tags.Contains("Fire") && !data.IsCrit)
        {
            data.Effectiveness = Reaction.Block;
            data.Damage = 0;
        }
    }

    public override void Cleanup()
    {
        Typocrypha.Keyboard.instance.RemoveEffect(affectedKeys);
        base.Cleanup();
    }
}
