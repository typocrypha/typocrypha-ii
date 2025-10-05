using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDahliaActI18 : AIComponent
{
    [SerializeField] private List<Spell> attackingSpells;
    [SerializeField] private Spell healSpell;
    [SerializeField] private Spell clearKeyEffectsSpell;
    [SerializeField] private float baseChargeTime;
    [SerializeField] private float chargeTimeVariance;

    private ATB3.ATBActor actor;
    private float charge;
    private float goal;

    protected override void Awake()
    {
        base.Awake();
        actor = GetComponent<ATB3.ATBActor>();
        UpdateGoal();
    }

    private void Update()
    {
        if (actor.IsPausedOrCasting())
            return;
        if((charge += (Time.deltaTime * Settings.GameplaySpeed)) >= goal)
        {
            charge = 0;
            UpdateGoal();
            DoCast();
        }
    }

    private void UpdateGoal()
    {
        float variance = (float)RandomUtils.RandomU.instance.RandomDouble() * chargeTimeVariance;
        if (RandomUtils.RandomU.instance.RandomBool())
        {
            goal = baseChargeTime + variance;
        }
        else
        {
            goal = baseChargeTime - variance;
        }
    }


    private void DoCast()
    {
        var player = Battlefield.instance.Player;
        if (RandomUtils.RandomU.instance.RollSuccess((1 - ((double)player.Health / player.Stats.MaxHP)) * 2))
        {
            AllyBattleBoxManager.instance.ShakeBattleBox();
            InsertCast(player.FieldPos, healSpell, null);
            return;
        }
        if (Typocrypha.Keyboard.instance.allEffects.Count >= RandomUtils.RandomU.instance.RandomInt(1, 8))
        {
            AllyBattleBoxManager.instance.ShakeBattleBox();
            InsertCast(player.FieldPos, clearKeyEffectsSpell, null);
            return;
        }
        var enemyChoices = new List<Caster>(Battlefield.instance.Enemies);
        enemyChoices.RemoveAll(IsNotValidTarget);
        if (enemyChoices.Count <= 0)
            return;
        var target = RandomUtils.RandomU.instance.Choice(enemyChoices);
        AllyBattleBoxManager.instance.ShakeBattleBox();
        InsertCast(target.FieldPos, RandomUtils.RandomU.instance.Choice(attackingSpells), null);
    }

    private bool IsNotValidTarget(Caster enemy)
    {
        return enemy.IsDeadOrFled || enemy.BStatus == Caster.BattleStatus.SpiritMode;
    }
}
