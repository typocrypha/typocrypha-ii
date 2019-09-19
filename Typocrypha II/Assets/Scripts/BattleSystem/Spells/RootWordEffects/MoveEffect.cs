using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RandomUtils;

public class MoveEffect : RootWordEffect
{
    public bool onlyMoveToUnoccupied;
    public List<Battlefield.Position> positions;
    public override CastResults Cast(Caster caster, Caster target, RootCastResults prevResults = null)
    {
        CastResults results = new CastResults(caster, target);
        results.Miss = false;
        results.DisplayDamage = false;
        var viablePositions = new List<Battlefield.Position>(positions);
        viablePositions.RemoveAll((p) => !p.IsLegal || p == target.FieldPos);
        if(onlyMoveToUnoccupied)
            viablePositions.RemoveAll((p) => Battlefield.instance.GetObject(p) != null);
        if (viablePositions.Count > 0)
        {
            var pos = RandomU.instance.Choice(viablePositions);
            Battlefield.instance.Move(target.FieldPos, pos, Battlefield.MoveOption.Swap);
        }
        return results;
    }
}
