using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAdrestiaTutorial : AIComponent
{
    [SerializeField] private Spell parrySpell;
    [SerializeField] private Spell riposteSpell;
    [SerializeField] private SpellList normalSpells;
    [SerializeField] private AudioClip warningSfx;

    private Spell tempSpell;
    private Coroutine parryCR;

    protected override void Awake()
    {
        base.Awake();
        ChangeSpell(normalSpells[RandomUtils.RandomU.instance.RandomInt(0, normalSpells.Count)]);
        StartCoroutine(ParryCR());
    }

    protected override void AddListeners()
    {
        RemoveListeners();
        caster.OnAfterCastResolved += AfterCastResolved;
        caster.OnCountered += OnCountered;
    }

    protected override void RemoveListeners()
    {
        caster.OnAfterCastResolved -= AfterCastResolved;
        caster.OnCountered -= OnCountered;
    }

    private void OnCountered(Caster arg1, bool fullCounter)
    {
        if (fullCounter)
        {
            SetSpell();
        }
    }

    private void AfterCastResolved(Spell s, Caster caster, bool hitTarget)
    {
        SetSpell();
    }

    private void SetSpell()
    {
        ChangeSpell(normalSpells[RandomUtils.RandomU.instance.RandomInt(0, normalSpells.Count)]);
        if (parryCR != null)
        {
            StopCoroutine(parryCR);
        }
        parryCR = StartCoroutine(ParryCR());
    }

    private IEnumerator ParryCR()
    {
        float timeLeft = caster.ChargeTime - caster.Charge;
        if (timeLeft <= 1)
            yield break;
        float time = 0;
        float goalTime = Mathf.Max(1f, (float)RandomUtils.RandomU.instance.RandomDouble() * Mathf.Min(timeLeft, 3));
        var actor = caster.GetComponent<ATB3.ATBActor>();
        var waitForEndOfFrame = new WaitForEndOfFrame();
        bool playWarning = true;
        while (time < goalTime)
        {
            if (!actor.PH.Paused)
            {
                time += Time.deltaTime;
                if(playWarning && goalTime - time < 0.25f)
                {
                    AudioManager.instance.PlaySFX(warningSfx);
                    playWarning = false;
                }
            }
            yield return waitForEndOfFrame;
        }
        tempSpell = caster.Spell;
        caster.Spell = parrySpell;
        caster.OnBeforeHitResolved -= Parry;
        caster.OnBeforeHitResolved += Parry;
        time = 0;
        goalTime = Math.Min((caster.ChargeTime - caster.Charge) - 0.1f, 1.5f);
        while (time < goalTime)
        {
            if (!actor.PH.Paused)
            {
                time += Time.deltaTime;
            }
            yield return waitForEndOfFrame;
        }
        caster.Spell = tempSpell;
        caster.OnBeforeHitResolved -= Parry;
        parryCR = StartCoroutine(ParryCR());
    }

    private void Parry(RootWordEffect effect, Caster caster, Caster target, RootCastData spellData, CastResults data)
    {
        data.Effectiveness = Reaction.Block;
        data.Damage = 0;
        data.StaggerDamage = 0;
        InsertCast(caster.FieldPos, riposteSpell, null);
    }
}
