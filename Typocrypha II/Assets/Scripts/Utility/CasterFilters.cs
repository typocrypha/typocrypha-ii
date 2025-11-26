using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CasterFilters
{

    public static bool IsSpiritMode(Caster caster) => caster.IsSpiritMode;
    public static bool IsNotSpiritMode(Caster caster) => !caster.IsSpiritMode;
}
