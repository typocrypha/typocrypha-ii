using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//Contains targeting data and associated targeting modification methods
[System.Serializable]
public class TargetData
{
    public enum Type
    {
        TargetedPattern,
        AbsolutePattern,
        CasterCenteredPattern,
        Self,
        Target,
        Allies,
        AlliesAndSelf,
    }

    public BoolMatrix2D pattern = new BoolMatrix2D(2, 3);
    public Type type = Type.TargetedPattern;

    public List<Battlefield.Position> Target(Battlefield.Position casterPos, Battlefield.Position targetPos)
    {
        List<Battlefield.Position> ret = new List<Battlefield.Position>();

        #region Pattern-Based Targeting

        if (PatternApplies(type))
        {
            #region Calculate Column Shift and Flipping
            // Flip the pattern if the caster is on the upper row
            bool flip = casterPos.Row == 0;
            IntRange colRange = new IntRange(0, pattern.Columns - 1);
            int colShift = 0;
            if (type == Type.TargetedPattern)
            {
                // If targeting your own row, invert the current flip
                flip ^= (casterPos.Row == targetPos.Row);
                colShift = targetPos.Col - 1;
            }
            else if (type == Type.CasterCenteredPattern)
            {
                colShift = casterPos.Col - 1;
            }
            #endregion

            #region Find Targets
            colRange.Shift(-colShift);
            colRange.Limit(0, pattern.Columns - 1);
            var targets = flip ? pattern.RowsFlipped() : pattern;
            for (int row = 0; row < targets.Rows; ++row)
                foreach (int col in colRange)
                    if (targets[row, col])
                        ret.Add(new Battlefield.Position(row, col + colShift));
            #endregion

            return ret;
        }

        #endregion

        #region Class and State based targeting

        if (type == Type.Self)
            ret.Add(casterPos);
        else if (type == Type.Target)
            ret.Add(targetPos);
        else if (type == Type.Allies || type == Type.AlliesAndSelf)
        {
            var caster = Battlefield.instance.GetCaster(casterPos);
            if(caster != null)
            {
                if (caster.CasterClass == Caster.Class.Player)
                {
                    ret.AddRange(Battlefield.instance.Allies.Select((a) => a.FieldPos));
                    if (type == Type.AlliesAndSelf)
                        ret.Add(casterPos);
                }
                else if (caster.CasterClass == Caster.Class.PartyMember)
                {
                    ret.AddRange(Battlefield.instance.Allies.Select((a) => a.FieldPos));
                    ret.Add(Battlefield.instance.Player.FieldPos);
                    if (type != Type.AlliesAndSelf)
                        ret.Remove(casterPos);
                }
                else if (caster.CasterClass == Caster.Class.Other)
                {
                    ret.AddRange(Battlefield.instance.Casters.Where((c) => caster.CasterState == c.CasterState).Select((c) => c.FieldPos));
                    if (type != Type.AlliesAndSelf)
                        ret.Remove(casterPos);
                }
            }
        }

        #endregion

        return ret;
    }

    public static bool PatternApplies(Type type) => type == Type.TargetedPattern 
        || type == Type.AbsolutePattern || type == Type.CasterCenteredPattern; 
}
