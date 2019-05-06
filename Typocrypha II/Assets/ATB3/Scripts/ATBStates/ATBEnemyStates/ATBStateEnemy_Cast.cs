using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    public class ATBStateEnemy_Cast : ATBState
    {
        // The ID for this specific ATBState
        public override ATBStateID StateID { get { return ATBStateID.Cast; } }

        // Call upon entering given state
        public override void OnEnter()
        {
            Owner.StartCoroutine(CastAndExit());
            ((ATBEnemy)this.Owner).GetComponent<Animator>().SetTrigger("Cast");
            timer = 0.0f;
        }

        // Call on fixed update while in given state
        public override void OnUpdate()
        {

        }

        // Call upon exiting given state
        public override void OnExit()
        {
            //Debug.Log("ENEMY " + this.Owner.actorName + " has EXITED the CAST state!");
        }

        private IEnumerator CastAndExit()
        {
            var caster = Owner.GetComponent<Caster>();
            var AI = Owner.GetComponent<CasterAI>();
            yield return SpellManager.instance.Cast(caster.Spell, caster, caster.TargetPos);
            AI.OnAfterCast?.Invoke();
            Source.PerformTransition(ATBTransition.ToAfterCast);
        }

    }
}
