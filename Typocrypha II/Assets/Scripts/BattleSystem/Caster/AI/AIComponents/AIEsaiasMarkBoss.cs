using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIEsaiasMarkBoss : AIEsaiasMark
{
    protected override bool IsNotValidTarget(Caster caster)
    {
        return base.IsNotValidTarget(caster) || !caster.HasTag("Commander") || RuleFriendshipFrog.ProtectsCaster(caster);
    }
}
