using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BEConditionNumberOfUnits : BEConditionNumberComparison
{
    [SerializeField] private Caster.State casterState = Caster.State.Hostile; 

    [SerializeField] private List<Caster.BattleStatus> statuses = new List<Caster.BattleStatus>() { Caster.BattleStatus.Normal };

    protected override int Number => Battlefield.instance.Casters.Count(IsValidCaster);
    private bool IsValidCaster(Caster c)
    {
        return c.CasterState == casterState && statuses.Contains(c.BStatus);
    }
}
