using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStatePlayer_Cast : ATBState
    {
        // The ID for this specific ATBState
        public override ATBStateID StateID { get { return ATBStateID.Cast; } }
        private float timer = 0.0f;

        // Call upon entering given state
        public override void OnEnter()
        {
            //Debug.Log("PLAYER " + this.Owner.actorName + " has ENTERED the CAST state!");
            timer = 0.0f;
            var casterComponent = Owner.GetComponent<Caster>();
            //SpellManager.instance.Cast((Owner as ATBPlayer).currSpell, casterComponent, casterComponent.TargetPos);
            SpellManager.instance.Cast(casterComponent.Spell, casterComponent, casterComponent.TargetPos);
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {
            timer += Time.deltaTime;
            if (timer >= 2.0f)
                Source.PerformTransition(ATBTransition.ToAfterCast);
            return;
        }

        // Call upon exiting given state
        public override void OnExit()
        {
            //Debug.Log("PLAYER " + this.Owner.actorName + " has EXITED the CAST state!");
        }
    }
}
