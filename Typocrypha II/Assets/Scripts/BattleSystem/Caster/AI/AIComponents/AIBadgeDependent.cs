using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBadgeDependent : AICycleSpells
{
    [SerializeField] private SpellList altSpells;
    [SerializeField] private BadgeWord altBadge;

    protected override SpellList Spells => PlayerDataManager.Equipment.IsBadgeEquipped(altBadge) ? altSpells : base.Spells;
}
