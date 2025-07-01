using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseShowInputPromptEffect : RootWordEffect
{
    [SubSO("Fail Effect")]
    public RootWordEffect onFail;
    public override CastResults Cast(Caster caster, Caster target, RootCastData spellData, Damage.DamageModifier mod, RootCastResults prevResults = null)
    {
        Initialize(caster, target, spellData, mod.specialModifier, prevResults);
        IEnumerator OnPromptComplete(bool succeeded)
        {
            if (succeeded)
            {
                yield break;
            }
            var failResults = onFail.Cast(caster, target, new RootCastData(new Spell(), new List<RootWord>(), 0), mod);
            float waitTime = SpellFxManager.instance.PlayResultsPopup(failResults, Battlefield.instance.GetSpaceScreenSpace(target.FieldPos), Battlefield.instance.GetSpaceScreenSpace(caster.FieldPos));
            if(waitTime > 0)
            {
                yield return new WaitForSeconds(waitTime);
            }
        }
        SpellManager.instance.LogPromptPopup(Title, Prompt, Time, OnPromptComplete);
        return InitializeCastResults(caster, target, mod);
    }

    protected virtual void Initialize(Caster caster, Caster target, RootCastData spellData, Damage.SpecialModifier mod, RootCastResults prevResults) { }

    protected abstract string Title { get; }
    protected abstract string Prompt { get; }
    protected abstract float Time { get; }
}
