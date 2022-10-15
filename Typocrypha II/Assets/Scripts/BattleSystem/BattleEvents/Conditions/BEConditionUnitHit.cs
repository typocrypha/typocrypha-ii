using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BEConditionUnitHit : BattleEventCondition
{
    public enum UnitSelectType
    {
        Position,
        Name,
    }

    [SerializeField] private UnitSelectType selectionType;
    [SerializeField] private Battlefield.Position position;
    [SerializeField] private string unitName;

    private bool conditionFulfilled;

    protected override void AddEventHandlers()
    {
        base.AddEventHandlers();
        if(selectionType == UnitSelectType.Position)
        {
            var caster = Battlefield.instance.GetCaster(position);
            if(caster == null)
            {
                Debug.LogError($"BECondition {name} is attempting to attach event to null caster");
                return;
            }
            caster.OnAfterHitResolved += CheckHitTopLevel;
        }
        else if(selectionType == UnitSelectType.Name)
        {
            foreach(var caster in Battlefield.instance.Casters)
            {
                if(caster.DisplayName == unitName)
                {
                    caster.OnAfterHitResolved += CheckHitTopLevel;
                }
            }
        }
    }

    protected override void RemoveEventHandlers()
    {
        base.RemoveEventHandlers();
        if (selectionType == UnitSelectType.Position)
        {
            var caster = Battlefield.instance.GetCaster(position);
            if (caster == null)
            {
                Debug.LogError($"BECondition {name} is attempting to remove event handler from null caster");
                return;
            }
            caster.OnAfterHitResolved -= CheckHitTopLevel;
        }
        else if (selectionType == UnitSelectType.Name)
        {
            foreach (var caster in Battlefield.instance.Casters)
            {
                if (caster.DisplayName == unitName)
                {
                    caster.OnAfterHitResolved -= CheckHitTopLevel;
                }
            }
        }
    }

    public override bool Check()
    {
        return conditionFulfilled;
    }

    private void CheckHitTopLevel(RootWordEffect effect, Caster caster, Caster self, RootCastData spellData, CastResults data)
    {
        if(CheckHit(effect, caster, self, spellData, data))
        {
            conditionFulfilled = true;
            RemoveEventHandlers();
        }
    }

    protected abstract bool CheckHit(RootWordEffect effect, Caster caster, Caster self, RootCastData spellData, CastResults data);
}
