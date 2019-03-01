using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//Contains targeting data and associated targeting modification methods
[System.Serializable]
public class TargetData
{
    public enum Type
    {
        Targeted,
        Absolute,
        CasterCentered,
    }

    public BoolMatrix2D pattern = new BoolMatrix2D(2, 3);
    public Type type = Type.Targeted;

    public List<Battlefield.Position> target(Battlefield.Position casterPos, Battlefield.Position targetPos)
    {
        List<Battlefield.Position> ret = new List<Battlefield.Position>();

        #region Calculate Column Shift and Flipping
        bool flip = casterPos.Row == 0; //True if on enemy side, false if not
        int colShift = 0;
        if (type == Type.Targeted)
        {
            flip = flip ^ (casterPos.Row == targetPos.Row);
            colShift = 1 - (targetPos.Col);
        }
        else if (type == Type.CasterCentered)
            colShift = 1 - (targetPos.Col);
        #endregion

        #region Find Targets
        BoolMatrix2D targets = flip ? pattern.rotated180() as BoolMatrix2D : pattern;
        int startCol = System.Math.Max(0, colShift);
        int endCol = System.Math.Min(colShift, targets.Columns);
        for (int row = 0; row < targets.Rows; ++row)
            for (int col = startCol; col < endCol; ++col)
                if (targets[row, col])
                    ret.Add(new Battlefield.Position(row, col - colShift));
        #endregion

        return ret;
    }
}
