using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleBurningJustice : Rule
{
    public override string DisplayName => "Burning Justice";

    public override int CooldownModifier(SpellWord word)
    {
        return 2;
    }
}
