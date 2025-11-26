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
        SpritModeAllies,
    }

    public BoolMatrix2D pattern = new BoolMatrix2D(2, 3);
    public Type type = Type.TargetedPattern;

    public IEnumerable<Battlefield.Position> Target(Battlefield.Position casterPos, Battlefield.Position targetPos)
    {
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
                        yield return new Battlefield.Position(row, col + colShift);
            #endregion

            yield break;
        }

        #endregion

        #region Class and State based targeting

        if (type == Type.Self)
        {
            yield return casterPos;
        }
        else if (type == Type.Target)
        {
            yield return targetPos;
        }
        else if (type == Type.Allies || type == Type.AlliesAndSelf)
        {
            var caster = Battlefield.instance.GetCaster(casterPos);
            if(caster != null)
            {
                foreach(var ally in GetAllies(caster, casterPos, null))
                {
                    yield return ally;
                }
                // Add self if appropriate
                if (type == Type.AlliesAndSelf)
                {
                    yield return casterPos;
                }
            }
        }
        else if (type == Type.SpritModeAllies || type == Type.SpiritModeAlliesAndSelf)
        {
            var caster = Battlefield.instance.GetCaster(casterPos);
            if (caster != null)
            {
                foreach (var ally in GetAllies(caster, casterPos, IsSpiritMode))
                {
                    yield return ally;
                }
                // Add self if appropriate
                if (type == Type.SpiritModeAlliesAndSelf)
                {
                    yield return casterPos;
                }
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
                    yield return new Battlefield.Position(row, col);
                }
            }
        }

        #endregion
    }

    private bool IsSpiritMode(Caster caster) => caster.BStatus == Caster.BattleStatus.SpiritMode;

    private IEnumerable<Battlefield.Position> GetAllies(Caster caster, Battlefield.Position casterPos, System.Predicate<Caster> filter)
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
                yield return ally.FieldPos;
            }
        }
        else if (caster.CasterState == Caster.State.Ally)
        {
            yield return Battlefield.instance.Player.FieldPos;
            foreach (var ally in Battlefield.instance.Casters)
            {
                if (ally.CasterState != Caster.State.Ally)
                    continue;
                if (ally.IsDeadOrFled || ally.FieldPos == casterPos || (filter != null && !filter(ally)))
                    continue;
                yield return ally.FieldPos;
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
                yield return enemy.FieldPos;
            }
        }
    }

    public static bool PatternApplies(Type type) => type == Type.TargetedPattern 
        || type == Type.AbsolutePattern || type == Type.CasterCenteredPattern; 
}
