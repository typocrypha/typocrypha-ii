using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIEsaiasMark : AIAllyRandomTimer
{
    [SerializeField] private Spell followUpSpell;
    [SerializeField] private float cooldownSeconds;
    [SerializeField] private double randomAttackChance = 0.33;
    private bool onCooldown;
    private Coroutine cooldownCR;

    protected override void AddListeners()
    {
        RemoveListeners();
        Battlefield.instance.Player.OnAfterCastResolved += FollowUp;
        Battlefield.instance.Player.OnWaveStart += OnWaveStart;
    }

    protected override void RemoveListeners()
    {
        Battlefield.instance.Player.OnAfterCastResolved -= FollowUp;
        Battlefield.instance.Player.OnWaveStart -= OnWaveStart;
    }

    private Coroutine OnWaveStart(Caster arg)
    {
        onCooldown = false;
        StopCooldownCR();
        return null;
    }

    protected override void DoAction()
    {
        if (!RandomUtils.RandomU.instance.RollSuccess(randomAttackChance))
            return;
        CastAtRandomTarget(Battlefield.instance.Enemies, followUpSpell, true);
    }

    protected override bool IsNotValidTarget(Caster caster)
    {
        return base.IsNotValidTarget(caster) || caster.Health <= 10 || caster.HasTag("Targeted");
    }

    private void FollowUp(Spell s, Caster caster, bool hitTarget)
    {
        if (!hitTarget || onCooldown)
            return;
        var target = Battlefield.instance.GetCaster(caster.TargetPos);
        if (target == null || IsNotValidTarget(target))
            return;
        if((target.Countered || target.Stunned))
        {
            AllyBattleBoxManager.instance.ShakeBattleBox();
            onCooldown = true;
            InsertCast(target.FieldPos, followUpSpell, true, StartCooldown);
        }
    }

    private void StopCooldownCR()
    {
        if (cooldownCR != null)
        {
            StopCoroutine(cooldownCR);
            cooldownCR = null;
        }
    }
    private void StartCooldown()
    {
        StopCooldownCR();
        cooldownCR = StartCoroutine(Cooldown());
    }
    private IEnumerator Cooldown()
    {
        float time = 0;
        while(time < cooldownSeconds)
        {
            if (actor.IsPausedOrCasting())
                yield return null;
            time += Time.deltaTime;
            yield return null;
        }
        onCooldown = false;
    }
}
