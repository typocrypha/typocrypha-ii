using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIEsaiasMark : AIAllyRandomTimer
{
    [SerializeField] private Spell followUpSpell;
    [SerializeField] private float cooldownSeconds;
    private HashSet<Caster> alreadyTargeted = new HashSet<Caster>();
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
        alreadyTargeted.Clear();
        onCooldown = false;
        if(cooldownCR != null)
        {
            StopCoroutine(cooldownCR);
            cooldownCR = null;
        }
        return null;
    }

    protected override void DoAction()
    {
        if (!RandomUtils.RandomU.instance.RollSuccess(0.33))
            return;
        CastAtRandomTarget(Battlefield.instance.Enemies, followUpSpell, true, MarkTargeted);
    }

    protected override bool IsNotValidTarget(Caster caster)
    {
        return base.IsNotValidTarget(caster) || alreadyTargeted.Contains(caster) || caster.Health <= 10;
    }

    private void FollowUp(Spell s, Caster caster, bool hitTarget)
    {
        if (!hitTarget || onCooldown)
            return;
        var target = Battlefield.instance.GetCaster(caster.TargetPos);
        if (target == null || IsNotValidTarget(target))
            return;
        if((target.Countered || target.Stunned) && RandomUtils.RandomU.instance.RollSuccess(0.9))
        {
            AllyBattleBoxManager.instance.ShakeBattleBox();
            MarkTargeted(target);
            onCooldown = true;
            QueueCast(target.FieldPos, followUpSpell, true, StartCooldown);
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
        }
        onCooldown = false;
    }

    private void MarkTargeted(Caster target)
    {
        if (alreadyTargeted.Contains(target))
            return;
        alreadyTargeted.Add(target);
    }
}
