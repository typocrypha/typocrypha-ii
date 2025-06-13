using ATB3;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A base class to organize all AI components.
/// Contains a protected reference to the caster.
/// This reference will be initialized if InitializeBase is called
/// </summary>
public abstract class AIComponent : MonoBehaviour
{
    protected Caster caster;

    protected virtual void Awake()
    {
        caster = GetComponent<Caster>();
        // Init targeting position
        caster.TargetPos = Battlefield.instance.Player.FieldPos;
        AddListeners();
    }

    private void OnDestroy()
    {
        RemoveListeners();
    }

    protected virtual void AddListeners()
    {

    }

    protected virtual void RemoveListeners()
    {

    }

    protected void ChangeSpell(Spell spell)
    {
        caster.Spell = spell;
        caster.ChargeTime = spell.CastTime;
        caster.Charge = 0;
    }

    protected void InsertCast(Battlefield.Position spellTargetPosition, Spell spellToCast, System.Action onComplete, string messageOverride = null)
    {
        var spell = spellToCast;
        var targetPos = spellTargetPosition;
        bool topLevel = !ATBManager.instance.ProcessingActions;
        Coroutine CastFn()
        {
            return SpellManager.instance.Cast(spell, caster, targetPos, messageOverride, topLevel);
        }
        ATBManager.instance.InsertSolo(new ATBManager.ATBAction() { Actor = GetComponent<ATBActor>(), Action = CastFn, OnComplete = onComplete });
    }
}
