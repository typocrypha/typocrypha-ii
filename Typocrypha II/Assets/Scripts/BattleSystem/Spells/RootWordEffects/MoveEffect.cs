using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RandomUtils;

public class MoveEffect : RootWordEffect
{
    public enum Filter
    {
        None,
        OnlyUnoccupied,
        OnlyOccupied,
    }
    public Filter validSpaceFilter;
    public List<Battlefield.Position> positions;

    public override CastResults Cast(Caster caster, Caster target, RootCastData spellData, Damage.DamageModifier mod, RootCastResults prevResults = null)
    {
        var results = InitializeCastResults(caster, target, mod);
        var viablePositions = new List<Battlefield.Position>(positions);
        viablePositions.RemoveAll((p) => !p.IsLegal || p == target.FieldPos);
        if(validSpaceFilter == Filter.OnlyUnoccupied)
        {
            viablePositions.RemoveAll(Battlefield.instance.IsOccupied);
        }
        else if(validSpaceFilter == Filter.OnlyOccupied)
        {
            viablePositions.RemoveAll(Battlefield.instance.IsEmpty);
        }
        if (viablePositions.Count > 0)
        {
            results.Miss = false;
            var pos = RandomU.instance.Choice(viablePositions);
            Battlefield.instance.Move(target.FieldPos, pos, Battlefield.MoveOption.Swap);
        }
        else
        {
            results.Miss = true;
        }
        return results;
    }
}
