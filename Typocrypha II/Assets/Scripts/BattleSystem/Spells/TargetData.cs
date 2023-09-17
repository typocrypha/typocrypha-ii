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
        SpiritModeAlliesAndSelf,
        EveryoneExceptSelf,
    }

    public BoolMatrix2D pattern = new BoolMatrix2D(2, 3);
    public Type type = Type.TargetedPattern;

    public List<Battlefield.Position> Target(Battlefield.Position casterPos, Battlefield.Position targetPos)
    {
        var ret = new List<Battlefield.Position>(6);

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
        {
            ret.Add(casterPos);
        }
        else if (type == Type.Target)
        {
            ret.Add(targetPos);
        }
        else if (type == Type.Allies || type == Type.AlliesAndSelf)
        {
            var caster = Battlefield.instance.GetCaster(casterPos);
            if(caster != null)
            {
                GetAllies(caster, casterPos, ref ret, null);
                // Add self if appropriate
                if (type == Type.AlliesAndSelf)
                {
                    ret.Add(casterPos);
                }
            }
        }
        else if (type == Type.SpiritModeAlliesAndSelf)
        {
            var caster = Battlefield.instance.GetCaster(casterPos);
            if (caster != null)
            {
                GetAllies(caster, casterPos, ref ret, IsSpiritMode);
                ret.Add(casterPos);
            }
        }
        else if(type == Type.EveryoneExceptSelf)
        {
            for (int row = 0; row < Battlefield.instance.Rows; ++row)
            {
                for (int col = 0; col < Battlefield.instance.Columns; ++col)
                {
                    if (row == casterPos.Row && col == casterPos.Col)
                        continue;
                    ret.Add(new Battlefield.Position(row, col));
                }
            }
        }

        #endregion

        return ret;
    }

    private bool IsSpiritMode(Caster caster) => caster.BStatus == Caster.BattleStatus.SpiritMode;

    private void GetAllies(Caster caster, Battlefield.Position casterPos, ref List<Battlefield.Position> ret, System.Predicate<Caster> filter)
    {
        // Logic for player, allies, and enemies. Other states have no defined allies
        if (caster.IsPlayer)
        {
            foreach (var ally in Battlefield.instance.Casters)
            {
                if (ally.CasterState != Caster.State.Ally)
                    continue;
                if (ally.IsDeadOrFled || (filter != null && !filter(ally)))
                    continue;
                ret.Add(ally.FieldPos);
            }
        }
        else if (caster.CasterState == Caster.State.Ally)
        {
            ret.Add(Battlefield.instance.Player.FieldPos);
            foreach (var ally in Battlefield.instance.Casters)
            {
                if (ally.CasterState != Caster.State.Ally)
                    continue;
                if (ally.IsDeadOrFled || ally.FieldPos == casterPos || (filter != null && !filter(ally)))
                    continue;
                ret.Add(ally.FieldPos);
            }
        }
        else if (caster.CasterState == Caster.State.Hostile)
        {
            foreach (var enemy in Battlefield.instance.Casters)
            {
                if (enemy.CasterState != Caster.State.Hostile)
                    continue;
                if (enemy.IsDeadOrFled || enemy.FieldPos == casterPos || (filter != null && !filter(enemy)))
                    continue;
                ret.Add(enemy.FieldPos);
            }
        }
    }

    public static bool PatternApplies(Type type) => type == Type.TargetedPattern 
        || type == Type.AbsolutePattern || type == Type.CasterCenteredPattern; 
}
