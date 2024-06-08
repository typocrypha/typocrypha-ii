using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadgeEffectAddTag : BadgeEffect
{
    [SerializeField] private CasterTag tag;

    public override void Equip(Player player)
    {
        player.AddTag(tag);
    }

    public override void Unequip(Player player)
    {
        player.RemoveTag(tag);
    }
}
