using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateEnemy_Cast : ATBState<ATBEnemy>
    {
        // Call upon entering given state
        public override void OnEnter()
        {
            var caster = Owner.Caster;
            if (caster.Spell == null)
            {
                Source.PerformTransition(ATBStateID.Charge);
                return;
            }
            var spell = caster.Spell;
            var targetPos = caster.TargetPos;
            bool topLevel = !ATBManager.instance.ProcessingActions;
            Coroutine Cast()
            {
                Owner.GetComponent<Animator>().SetTrigger("Cast");
                return SpellManager.instance.Cast(spell, caster, targetPos, null, topLevel);
            }
            Debug.LogWarning($"Caster {Owner.name} is entering cast");
            ATBManager.instance.QueueSolo(new ATBManager.ATBAction() { Actor = Owner, Action = Cast, OnComplete = CastComplete });
            // Trigger other enemies who are in precast
            if (topLevel)
            {
                var precastEnemies = new List<ATBEnemy>(Battlefield.instance.Columns);
                foreach (var actor in Battlefield.instance.Actors)
                {
                    if (actor.IsCurrentState(ATBStateID.PreCast) && actor is ATBEnemy atbEnemy)
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
            if (Owner.Caster.BStatus == Caster.BattleStatus.Dead)
            {
                Source.PerformTransition(ATBStateID.Dead);
            }
            else if (Owner.Caster.BStatus == Caster.BattleStatus.Fled)
            {
                Source.PerformTransition(ATBStateID.Fled);
            }
            else
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
