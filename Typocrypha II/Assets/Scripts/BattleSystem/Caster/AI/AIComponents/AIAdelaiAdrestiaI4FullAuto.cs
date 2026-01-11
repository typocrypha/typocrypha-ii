using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAdelaiAdrestiaI4FullAuto : AIAllyRandomTimer
{
    [SerializeField] private Spell defaultSpell;
    [SerializeField] private List<SpellWord> counterable;

    private readonly Battlefield.Position targetPos = new Battlefield.Position(0, 1);
    private readonly Spell spell = new Spell();
    private readonly List<SpellWord> wordChoices = new List<SpellWord>(3);

    private bool hasBeenParried;
    private int vengeanceCount = 0;

    protected override void DoAction()
    {
        var target = RandomUtils.RandomU.instance.Choice(Battlefield.instance.NonSpiritModeEnemies);
        wordChoices.Clear();
        foreach(var word in target.Spell)
        {
            if (counterable.Contains(word))
            {
                wordChoices.Add(word);
            }
        }
        if(wordChoices.Count > 0)
        {
            if (wordChoices.Count == 1 && caster.Health == caster.Stats.MaxHP)
                return;
            spell.Clear();
            spell.Add(RandomUtils.RandomU.instance.Choice(wordChoices));
            QueueCast(target.FieldPos, spell, true, null);
        }
        else if (target.Spell[0].Key == "parry")
        {
            if (!hasBeenParried && caster.Health > 75 && caster.Health < caster.Stats.MaxHP)
            {
                hasBeenParried = true;
                QueueCast(target.FieldPos, defaultSpell, true, null);
            }
        }
        else if(target.Spell[0].Key == "vengeance")
        {
            if(++vengeanceCount > 3)
            {
                vengeanceCount = 0;
                QueueCast(target.FieldPos, defaultSpell, true, null);
            }
        }
        else
        {
            QueueCast(target.FieldPos, defaultSpell, true, null);
        }
    }
}
