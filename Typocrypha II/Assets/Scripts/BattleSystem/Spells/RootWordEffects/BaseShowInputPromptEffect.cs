using System.Collections;
using System.Collections.Generic;

public abstract class BaseShowInputPromptEffect : RootWordEffect
{
    [SubSO("Fail Effect")]
    public RootWordEffect onFail;
    public override CastResults Cast(Caster caster, Caster target, RootCastData spellData, Damage.SpecialModifier mod, RootCastResults prevResults = null)
    {
        Initialize(caster, target, spellData, mod, prevResults);
        IEnumerator OnPromptComplete(bool succeeded)
        {
            if (succeeded)
            {
                yield break;
            }
            var failResults = onFail.Cast(caster, target, new RootCastData(new Spell(), new List<RootWord>(), 0), mod);
            yield return SpellFxManager.instance.PlayFullPopup(failResults, Battlefield.instance.GetSpaceScreenSpace(target.FieldPos), Battlefield.instance.GetSpaceScreenSpace(caster.FieldPos));
        }
        SpellManager.instance.LogPromptPopup(Title, Prompt, Time, OnPromptComplete);
        return new CastResults(caster, target)
        {
            DisplayDamage = false,
            Miss = false,
        };
    }

    protected virtual void Initialize(Caster caster, Caster target, RootCastData spellData, Damage.SpecialModifier mod, RootCastResults prevResults) { }

    protected abstract string Title { get; }
    protected abstract string Prompt { get; }
    protected abstract float Time { get; }
}
