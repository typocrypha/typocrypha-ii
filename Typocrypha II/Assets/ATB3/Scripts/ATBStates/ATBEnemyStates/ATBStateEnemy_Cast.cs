using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateEnemy_Cast : ATBState<ATBEnemy>
    {
        private static readonly int castHash = Animator.StringToHash("Cast");
        // Call upon entering given state
        public override void OnEnter()
        {
            var caster = Owner.Caster;
            if (caster.Spell == null)
            {
                Source.PerformTransition(ATBStateID.Charge);
                return;
            }
            var targetPos = caster.TargetPos;
            bool topLevel = !ATBManager.instance.ProcessingActions;
            bool enteredSpiritMode = false;
            void SpiritModeListener()
            {
                enteredSpiritMode = true;
            }
            if (caster.BStatus == Caster.BattleStatus.Normal)
            {
                caster.OnSpiritMode += SpiritModeListener;
            }

            Coroutine Cast()
            {
                caster.OnSpiritMode -= SpiritModeListener;
                if (enteredSpiritMode)
                {
                    caster.Charge = 0;
                    return null;
                }
                if (caster.IsInactive)
                {
                    return null;
                }
                if (caster.Stunned)
                {
                    Source.PerformTransition(ATBStateID.Stunned);
                    return null;
                }
                if (caster.Countered)
                {
                    caster.OnAfterCastResolved?.Invoke(caster.Spell, caster, false);
                    return null;
                }
                if (caster.Spell.PlayCastAnimation)
                {
                    Source.SetAnimation(castHash); // TODO: maybe none state
                }
                return SpellManager.instance.Cast(caster.Spell, caster, targetPos, null, topLevel);
            }
            ATBManager.instance.QueueSolo(new ATBManager.ATBAction() { Actor = Owner, Action = Cast, OnComplete = CastComplete });
            // Trigger other enemies who are in precast
            if (topLevel)
            {
                var precastEnemies = new List<ATBEnemy>(Battlefield.instance.Columns);
                foreach (var actor in Battlefield.instance.Actors)
                {
                    if (actor.IsCurrentState(ATBStateID.PreCast) && actor is ATBEnemy atbEnemy && !atbEnemy.Caster.Spell.IsRun)
                    {
                        precastEnemies.Add(atbEnemy);
                    }
                }
                precastEnemies.Sort(PreCastSorter);
                foreach(var enemy in precastEnemies)
                {
                    enemy.BaseStateMachine.PerformTransition(ATBStateID.Cast);
                }
            }
        }

        private static int PreCastSorter(ATBEnemy e1, ATBEnemy e2)
        {
            if (!(e1.BaseStateMachine.CurrentATBState is ATBStateEnemy_PreCast p1 && e2.BaseStateMachine.CurrentATBState is ATBStateEnemy_PreCast p2))
                return 0;
            return p2.Timer.CompareTo(p1.Timer);
        }

        private void CastComplete()
        {
            if (Source.CurrentStateID != ATBStateID.Cast)
                return;
            if (!CheckDeathOrRun(Owner.Caster))
            {
                Source.PerformTransition(ATBStateID.Charge);
            }
        }

        // Call on fixed update while in given state
        public override void OnUpdate() { }

        // Call upon exiting given state
        public override void OnExit() { }
    }
}
