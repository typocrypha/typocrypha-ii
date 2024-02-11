using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowInputPromptEffect : RootWordEffect
{
    public override CastResults Cast(Caster caster, Caster target, RootCastData spellData, Damage.SpecialModifier mod, RootCastResults prevResults = null)
    {
        IEnumerator OnPromptComplete(bool succeeded)
        {
            if (succeeded)
            {
                yield break;
            }
            target.Damage(10);
            yield return SpellFxManager.instance.PlayDamageNumber(10, Battlefield.instance.GetSpaceScreenSpace(target.FieldPos));
        }
        SpellManager.instance.LogPromptPopup("TYPETHIS", "TYPETHIS", 2.5f, OnPromptComplete);
        return new CastResults(caster, target)
        {
            DisplayDamage = false,
            Miss = false,
        };
    }


}
