using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public List<Battlefield.Position> Target(Battlefield.Position casterPos, Battlefield.Position targetPos)
    {
        List<Battlefield.Position> ret = new List<Battlefield.Position>();

        #region Calculate Column Shift and Flipping
        // Flip the pattern if the caster is on the upper row
        bool flip = casterPos.Row == 0; 
        IntRange colRange = new IntRange(0, pattern.Columns - 1);
        int colShift = 0;
        if (type == Type.Targeted)
        {
            // If targeting your own row, invert the current flip
            flip ^= (casterPos.Row == targetPos.Row);
            colShift = targetPos.Col - 1;
        }
        else if (type == Type.CasterCentered)
        {
            colShift = casterPos.Col - 1;
        }
        #endregion

        #region Find Targets
        colRange.Shift(-colShift);
        colRange.Limit(0, pattern.Columns - 1);
        var targets = flip ? pattern.RowsFlipped() : pattern;
        for (int row = 0; row < targets.Rows; ++row)
            foreach(int col in colRange)
                if (targets[row, col])
                    ret.Add(new Battlefield.Position(row, col + colShift));
        #endregion

        return ret;
    }
}
