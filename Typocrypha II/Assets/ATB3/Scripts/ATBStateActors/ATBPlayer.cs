using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    [RequireComponent(typeof(ATBStateMachine_Player))]
    public partial class ATBPlayer : ATBActor
    {
        public ATBStateMachine_Player StateMachine { get; private set; }
        public override IATBStateMachine BaseStateMachine => StateMachine;
        public Battlefield.Position SavedTargetPos { get; private set; }

        public override void Setup()
        {
            StateMachine = GetComponent<ATBStateMachine_Player>();
        }

        // Called when player enters a spell into the cast bar
        public void Cast(Battlefield.Position targetPos)
        {
            SavedTargetPos = new Battlefield.Position(targetPos);
            StateMachine.PerformTransition(ATBStateID.Cast);
        }

        public void InsertCast(Battlefield.Position spellTargetPosition, Spell spellToCast, System.Action onComplete, string messageOverride = null)
        {
            var caster = GetComponent<Caster>();
            var spell = spellToCast;
            var targetPos = spellTargetPosition;
            bool topLevel = !ATBManager.instance.ProcessingActions;
            Coroutine CastFn()
            {
                isCast = true;
                FaderManager.instance.FadeTargets(spell, caster.FieldPos, targetPos);
                return SpellManager.instance.CastAndCounter(spell, caster, targetPos, messageOverride, topLevel);
            }
            ATBManager.instance.InsertSolo(new ATBManager.ATBAction() { Actor = this, Action = CastFn, OnComplete = onComplete });
        }

    }
}

