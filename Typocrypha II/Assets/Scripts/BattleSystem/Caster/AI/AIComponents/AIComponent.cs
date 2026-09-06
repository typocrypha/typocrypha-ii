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

    protected void ChangeSpellRandom(IReadOnlyList<Spell> spells)
    {
        ChangeSpell(RandomUtils.RandomU.instance.Choice(spells));
    }

    protected void ChangeSpell(Spell spell)
    {
        caster.Spell = spell;
        caster.ChargeTime = spell.CastTime;
        caster.Charge = 0;
    }

    public bool CastAtRandomTarget(IEnumerable<Caster> targets, IReadOnlyList<Spell> spells, bool canCounter, System.Func<Caster, float> weightFn = null, System.Action<Caster> onTargetSelected = null)
    {
        return CastAtRandomTarget(targets, RandomUtils.RandomU.instance.Choice(spells), canCounter, weightFn, onTargetSelected);
    }

    protected bool CastAtRandomTarget(IEnumerable<Caster> targets, Spell spell, bool canCounter, System.Func<Caster, float> weightFn = null, System.Action<Caster> onTargetSelected = null)
    {
        var enemyChoices = weightFn == null ? new RandomUtils.WeightedSet<Caster>(targets) : new RandomUtils.WeightedSet<Caster>(targets, weightFn);
        enemyChoices.RemoveWhere(IsNotValidTarget);
        if (enemyChoices.Count <= 0)
            return false;
        var target = RandomUtils.RandomU.instance.Choice(enemyChoices);
        onTargetSelected?.Invoke(target);
        AllyBattleBoxManager.instance.ShakeBattleBox();
        QueueCast(target.FieldPos, spell, canCounter, null);
        return true;
    }

    protected virtual bool IsNotValidTarget(Caster caster)
    {
        return caster.IsInactive || caster.BStatus == Caster.BattleStatus.SpiritMode;
    }

    protected void InsertCast(Battlefield.Position spellTargetPosition, Spell spellToCast, bool canCounter = false, System.Action onComplete = null, string messageOverride = null)
    {
        var castFn = GetCastFunction(spellTargetPosition, spellToCast, canCounter, messageOverride);
        ATBManager.instance.InsertSolo(new ATBManager.ATBAction() { Actor = GetComponent<ATBActor>(), Action = castFn, OnComplete = onComplete });
    }

    protected void QueueCast(Battlefield.Position spellTargetPosition, Spell spellToCast, bool canCounter = false, System.Action onComplete = null, string messageOverride = null)
    {
        var castFn = GetCastFunction(spellTargetPosition, spellToCast, canCounter, messageOverride);
        ATBManager.instance.QueueSolo(new ATBManager.ATBAction() { Actor = GetComponent<ATBActor>(), Action = castFn, OnComplete = onComplete });
    }

    private System.Func<Coroutine> GetCastFunction(Battlefield.Position spellTargetPosition, Spell spellToCast, bool canCounter, string messageOverride)
    {
        var spell = spellToCast;
        var targetPos = spellTargetPosition;
        var counter = canCounter;
        bool topLevel = !ATBManager.instance.ProcessingActions;
        Coroutine CastFn()
        {
            // Cancel if stunned, dead/fled, or countered
            if (caster.Stunned || caster.IsInactive)
                return null;
            var actor = caster.GetComponent<ATBActor>();
            if (actor != null)
                actor.isCast = true;
            if (counter)
            {
                return SpellManager.instance.CastAndCounter(spell, caster, targetPos, messageOverride, topLevel);
            }
            return SpellManager.instance.Cast(spell, caster, targetPos, messageOverride, topLevel);
        }
        return CastFn;
    }
}
