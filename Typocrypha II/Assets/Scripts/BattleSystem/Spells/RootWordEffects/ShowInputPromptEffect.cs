using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowInputPromptEffect : RootWordEffect
{
    [SerializeField] private string title;
    [SerializeField] private string prompt;
    [SerializeField] private float time;
    [SubSO("Fail Effect")]
    public RootWordEffect onFail;
    public override CastResults Cast(Caster caster, Caster target, RootCastData spellData, Damage.SpecialModifier mod, RootCastResults prevResults = null)
    {
        IEnumerator OnPromptComplete(bool succeeded)
        {
            if (succeeded)
            {
                yield break;
            }
            var failResults = onFail.Cast(caster, target, new RootCastData(new Spell(), new List<RootWord>(), 0), mod);
            yield return SpellFxManager.instance.PlayFullPopup(failResults, Battlefield.instance.GetSpaceScreenSpace(target.FieldPos), Battlefield.instance.GetSpaceScreenSpace(caster.FieldPos));
        }
        SpellManager.instance.LogPromptPopup(title, prompt, time, OnPromptComplete);
        return new CastResults(caster, target)
        {
            DisplayDamage = false,
            Miss = false,
        };
    }


}
