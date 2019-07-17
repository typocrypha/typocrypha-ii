using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStatePlayer_Cast : ATBState<ATBPlayer>
    {
        // The ID for this specific ATBState
        public override ATBStateID StateID { get { return ATBStateID.Cast; } }

        // Call upon entering given state
        public override void OnEnter()
        {
            //Debug.Log("PLAYER " + this.Owner.actorName + " has ENTERED the CAST state!");
            Owner.StartCoroutine(CastAndExit());
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {

        }

        // Call upon exiting given state
        public override void OnExit()
        {
            //Debug.Log("PLAYER " + this.Owner.actorName + " has EXITED the CAST state!");
        }

        private IEnumerator CastAndExit()
        {
            var caster = Owner.GetComponent<Caster>();
            yield return SpellManager.instance.Cast(caster.Spell, caster, caster.TargetPos);
            Source.PerformTransition(ATBTransition.ToAfterCast);
        }
    }
}
