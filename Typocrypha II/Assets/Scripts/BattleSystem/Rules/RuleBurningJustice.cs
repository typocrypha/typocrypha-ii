using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleBurningJustice : Rule
{
    public override int CooldownModifier(SpellWord word)
    {
        return 2;
    }
}
