using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDahliaActI18 : AIAllyRandomTimer
{
    [SerializeField] private List<Spell> attackingSpells;
    [SerializeField] private Spell healSpell;
    [SerializeField] private Spell clearKeyEffectsSpell;

    protected override void DoAction()
    {
        var player = Battlefield.instance.Player;
        if (RandomUtils.RandomU.instance.RollSuccess((1 - ((double)player.Health / player.Stats.MaxHP)) * 2))
        {
            AllyBattleBoxManager.instance.ShakeBattleBox();
            QueueCast(player.FieldPos, healSpell, true);
            return;
        }
        if (Typocrypha.Keyboard.instance.allEffects.Count >= RandomUtils.RandomU.instance.RandomInt(1, 8))
        {
            AllyBattleBoxManager.instance.ShakeBattleBox();
            QueueCast(player.FieldPos, clearKeyEffectsSpell, true);
            return;
        }
        CastAtRandomTarget(Battlefield.instance.Enemies, attackingSpells, true);
    }


}
