using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAdrestiaTutorial : AIComponent
{
    [SerializeField] private Spell parrySpell;
    [SerializeField] private Spell riposteSpell;
    [SerializeField] private Spell callAlliesSpell;
    [SerializeField] private Spell clearAndCallAlliesSpell;
    [SerializeField] private Spell enrageAlliesSpell;
    [SerializeField] private SpellList normalSpells;
    [SerializeField] private AudioClip warningSfx;

    private Spell tempSpell;
    private Coroutine parryCR;

    protected override void Awake()
    {
        base.Awake();
        ChangeSpell(normalSpells[RandomUtils.RandomU.instance.RandomInt(0, normalSpells.Count)]);
        PrepareParry();
    }

    protected override void AddListeners()
    {
        RemoveListeners();
        caster.OnAfterCastResolved += AfterCastResolved;
        caster.OnCountered += OnCountered;
        ATB3.ATBManager.instance.OnExitSolo += AfterATBSequence;
    }

    protected override void RemoveListeners()
    {
        caster.OnAfterCastResolved -= AfterCastResolved;
        caster.OnCountered -= OnCountered;
        ATB3.ATBManager.instance.OnExitSolo -= AfterATBSequence;
    }

    private void OnCountered(Caster arg1, bool fullCounter)
    {
        if (!fullCounter)
        {
            return;
        }
        if (Battlefield.instance.ValidReinforcementPositions.Count > 0)
        {
            QueueCast(caster.FieldPos, callAlliesSpell, null, $"{caster.DisplayName} summons an ally!");
            return;
        }
        // Check for clearable allies
        foreach (var other in Battlefield.instance.Casters)
        {
            if (other != caster && other.CasterState == caster.CasterState && other.BStatus == Caster.BattleStatus.SpiritMode)
            {
                QueueCast(caster.FieldPos, clearAndCallAlliesSpell, null, $"{caster.DisplayName} summons an ally!");
                return;
            }
        }
        bool allAlliesEnraged = true;
        foreach (var other in Battlefield.instance.Casters)
        {
            if (other != caster && other.CasterState == caster.CasterState && !other.HasTag("Enraged"))
            {
                allAlliesEnraged = false;
                break;
            }
        }
        if (allAlliesEnraged)
        {
            return;
        }
        QueueCast(caster.FieldPos, enrageAlliesSpell, null, $"{caster.DisplayName}'s allies were filled with vengeance!");
    }

    private void AfterCastResolved(Spell s, Caster caster, bool hitTarget)
    {
        if (caster.BStatus == Caster.BattleStatus.SpiritMode || caster.Countered)
            return;
        SetSpell();
    }

    private void AfterATBSequence()
    {
        if (caster.Countered)
        {
            SetSpell();
        }
    }

    private void SetSpell()
    {
        CancelParry();
        ChangeSpell(normalSpells[RandomUtils.RandomU.instance.RandomInt(0, normalSpells.Count)]);
        PrepareParry();
    }

    private void PrepareParry()
    {
        if (caster.IsDeadOrFled)
            return;
        parryCR = StartCoroutine(ParryCR());
    }

    private IEnumerator ParryCR()
    {
        float timeLeft = caster.ChargeTime - caster.Charge;
        if (timeLeft <= 1)
            yield break;
        float time = 0;
        float hpFactor = Mathf.Min(1f, ((float)caster.Health / caster.Stats.MaxHP) * 2f);
        float goalTime = Mathf.Max(Mathf.Max(0.33f, 1f * hpFactor), (float)RandomUtils.RandomU.instance.RandomDouble() * hpFactor * Mathf.Min(timeLeft, 3));
        var actor = caster.GetComponent<ATB3.ATBActor>();
        var waitForEndOfFrame = new WaitForEndOfFrame();
        var waitForUnPause = new WaitWhile(actor.IsPausedOrCasting);
        bool playWarning = true;
        while (time < goalTime)
        {
            if (actor.IsPausedOrCasting())
            {
                yield return waitForUnPause;
            }
            time += Time.deltaTime;
            if (playWarning && goalTime - time < 0.33f)
            {
                AudioManager.instance.PlaySFX(warningSfx);
                playWarning = false;
            }
            yield return waitForEndOfFrame;
        }
        if (actor.IsPausedOrCasting())
        {
            yield return waitForUnPause;
        }
        StartParry();
        time = 0;
        goalTime = Math.Min((caster.ChargeTime - caster.Charge) - 0.1f, 1f + (float)RandomUtils.RandomU.instance.RandomDouble() * 0.25f);
        while (time < goalTime)
        {
            if (actor.IsPausedOrCasting())
            {
                yield return waitForUnPause;
            }
            time += Time.deltaTime;
            yield return waitForEndOfFrame;
        }
        if (actor.IsPausedOrCasting())
        {
            yield return waitForUnPause;
        }
        EndParry();
        PrepareParry();
    }

    private void StartParry()
    {
        tempSpell = caster.Spell;
        caster.Spell = parrySpell;
        caster.ui.SetTextColor(Color.red);
        caster.OnBeforeHitResolved -= Parry;
        caster.OnBeforeHitResolved += Parry;
    }

    private void EndParry()
    {
        caster.ui.SetTextColor(Color.white);
        if(tempSpell != null)
        {
            caster.Spell = tempSpell;
            tempSpell = null;
        }
        caster.OnBeforeHitResolved -= Parry;
    }

    private void CancelParry()
    {
        if (parryCR != null)
        {
            StopCoroutine(parryCR);
            parryCR = null;
            EndParry();
        }
    }

    private void Parry(RootWordEffect effect, Caster caster, Caster target, RootCastData spellData, CastResults data)
    {
        data.Effectiveness = Reaction.Block;
        data.Damage = 0;
        data.StaggerDamage = 0;
        InsertCast(caster.FieldPos, riposteSpell);
    }
}
