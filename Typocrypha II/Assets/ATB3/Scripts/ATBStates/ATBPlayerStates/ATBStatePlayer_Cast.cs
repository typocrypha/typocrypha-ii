using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStatePlayer_Cast : ATBState<ATBPlayer>
    {
        // Call upon entering given state
        public override void OnEnter()
        {
            var caster = Owner.GetComponent<Caster>();
            Owner.InsertCast(Owner.SavedTargetPos, caster.Spell, CastComplete);
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

        private void CastComplete()
        {
            Source.PerformTransition(ATBStateID.Idle);
        }
    }
}
