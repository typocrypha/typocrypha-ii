using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateAlly_Cast : ATBState<ATBAlly>
    {
        // Call upon entering given state
        public override void OnEnter()
        {
            Owner.StartCoroutine(CastAndExit());
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {

        }

        // Call upon exiting given state
        public override void OnExit()
        {
            //Debug.Log("ALLY " + this.Owner.actorName + " has EXITED the CAST state!");
        }

        private IEnumerator CastAndExit()
        {
            yield return SpellManager.instance.Cast(Owner.Caster.Spell, Owner.Caster, Owner.Caster.TargetPos);
            Source.PerformTransition(ATBStateID.AfterCast);
        }
    }
}
