using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FormulaFinishingBlow : CustomFormula
{
    public List<CasterTag> tags = new List<CasterTag>();
    public bool finishStunned = false;
    public float finishMultiplier;
    public override CastResults Apply(DamageEffect effect, Caster caster, Caster target, bool crit, RootCastResults prevResults = null)
    {
        var results = Damage.Standard(effect, caster, target, crit);

        if ((finishStunned && target.Stunned) || tags.Any((t) => target.HasTag(t)))
        {
            if (Damage.MoveDoesDamage(results.Effectiveness))
            {
                results.StaggerDamage = target.Stagger;
                results.Damage *= finishMultiplier;
                LogMessage(target.DisplayName + " was dealt a finishing blow!");
            }
            else
                LogMessage("The finishing blow was prevented!");
        }
        else
            LogMessage(target.DisplayName + " wasn't vulnerable.");

        Damage.ApplyStandard(results, effect, caster, target);
        return results;
    }
}
