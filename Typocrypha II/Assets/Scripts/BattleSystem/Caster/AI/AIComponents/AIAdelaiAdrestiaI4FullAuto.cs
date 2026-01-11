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

    protected override void DoAction()
    {
        var target = Battlefield.instance.GetCaster(targetPos);
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
            spell.Clear();
            spell.Add(RandomUtils.RandomU.instance.Choice(wordChoices));
            QueueCast(target.FieldPos, spell, true, null);
        }
        else
        {
            QueueCast(target.FieldPos, defaultSpell, true, null);
        }
    }
}
