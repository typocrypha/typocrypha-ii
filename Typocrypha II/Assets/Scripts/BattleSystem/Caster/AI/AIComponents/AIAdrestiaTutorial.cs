using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAdrestiaTutorial : AIComponent
{
    private enum Phase
    {
        Intro,
        Vengeance,
        Bloodlust,
        Hidden,
    }
    [SerializeField] private SpellList introSpells;
    [SerializeField] private Spell vengeanceSpell;
    [SerializeField] private Spell vengeanceSpell2;
    [SerializeField] private Spell parrySpell;
    [SerializeField] private Spell riposteSpell;
    [SerializeField] private Spell callAlliesSpell;
    [SerializeField] private Spell clearAndCallAlliesSpell;
    [SerializeField] private Spell enrageAlliesSpell;
    [SerializeField] private SpellList normalSpells;
    [SerializeField] private AudioClip warningSfx;


    private Spell tempSpell;
    private Coroutine parryCR;
    private Phase phase = Phase.Intro;
    private int bloodlustCount = 0;

    protected override void Awake()
    {
        base.Awake();
        ChangeSpellRandom(introSpells);
    }

    protected override void AddListeners()
    {
        RemoveListeners();
        caster.OnAfterCastResolved += AfterCastResolved;
        caster.OnCountered += OnCountered;
        caster.OnUnstunned += OnUnstunned;
        ATB3.ATBManager.instance.OnExitSolo += AfterATBSequence;
        BattleManager.instance.OnBattleEventTriggered += OnBattleEventTriggered;
    }

    protected override void RemoveListeners()
    {
        caster.OnAfterCastResolved -= AfterCastResolved;
        caster.OnCountered -= OnCountered;
        caster.OnUnstunned -= OnUnstunned;
        ATB3.ATBManager.instance.OnExitSolo -= AfterATBSequence;
        BattleManager.instance.OnBattleEventTriggered -= OnBattleEventTriggered;
    }

    private void OnCountered(Caster arg1, bool fullCounter)
    {
        if (!fullCounter)
        {
            return;
        }
        if (phase == Phase.Intro)
        {
            return;
        }
        if(phase == Phase.Bloodlust)
        {
            bloodlustCount++;
        }
        if (Battlefield.instance.ValidReinforcementPositions.Count > 0)
        {
            QueueCast(caster.FieldPos, callAlliesSpell, false, null, $"{caster.DisplayName} summons an ally!");
            return;
        }
        // Check for clearable allies
        foreach (var other in Battlefield.instance.Casters)
        {
            if (other != caster && other.CasterState == caster.CasterState && other.BStatus == Caster.BattleStatus.SpiritMode)
            {
                QueueCast(caster.FieldPos, clearAndCallAlliesSpell, false, null, $"{caster.DisplayName} summons an ally!");
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
        QueueCast(caster.FieldPos, enrageAlliesSpell, false, null, $"{caster.DisplayName}'s allies were filled with vengeance!");
    }

    private void OnUnstunned()
    {
        if(phase == Phase.Vengeance)
        {
            phase = Phase.Hidden;
            SetSpell();
        }
    }


    private void AfterCastResolved(Spell s, Caster caster, bool hitTarget)
    {
        if (caster.BStatus == Caster.BattleStatus.SpiritMode || caster.Countered)
            return;
        if(phase == Phase.Vengeance)
        {
            caster.Stagger = caster.Stats.MaxStagger;
            phase = Phase.Bloodlust;
        }
        else if(phase == Phase.Bloodlust && bloodlustCount >= 3)
        {
            ChangeSpell(vengeanceSpell2);
            CancelParry();
            return;
        }
        SetSpell();
    }

    private void AfterATBSequence()
    {
        if (phase == Phase.Intro || !caster.Countered)
            return;
        if (phase == Phase.Bloodlust && bloodlustCount >= 3)
        {
            ChangeSpell(vengeanceSpell2);
            CancelParry();
        }
        else
        {
            SetSpell();
        }
    }

    private void OnBattleEventTriggered(string id)
    {
        if (id == "AdrestiaVengeance")
        {
            caster.Stagger = 1;
            phase = Phase.Vengeance;
            ChangeSpell(vengeanceSpell);
        }
    }

    private void SetSpell()
    {
        CancelParry();
        if(phase == Phase.Intro)
        {
            ChangeSpellRandom(introSpells);
        }
        else
        {
            ChangeSpellRandom(normalSpells);
            PrepareParry();
        }
    }

    private void PrepareParry()
    {
        if (caster.IsInactive)
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
