using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BEConditionDecoded : BattleEventCondition
{
    [SerializeField] private string researchKey;
    private bool complete = false;
    public override bool Check()
    {
        return complete;
    }

    protected override void Initialize()
    {
        base.Initialize();
        CheckDecoded();
    }

    protected override void AddEventHandlers()
    {
        Battlefield.instance.Player.OnAfterCastResolved += CheckCast;
    }

    protected override void RemoveEventHandlers()
    {
        Battlefield.instance.Player.OnAfterCastResolved -= CheckCast;
    }

    private void CheckDecoded()
    {
        if (complete)
        {
            return;
        }
        if (PlayerDataManager.instance.researchData.IsDecoded(researchKey))
        {
            complete = true;
            RemoveEventHandlers();
        }
    }

    public void CheckCast(Spell s, Caster caster)
    {
        CheckDecoded();
    }
}
