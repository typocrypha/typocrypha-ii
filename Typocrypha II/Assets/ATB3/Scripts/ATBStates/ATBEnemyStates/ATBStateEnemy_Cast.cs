using System.Collections;
using System.Linq;
using UnityEngine;

namespace ATB3
{
    public class ATBStateEnemy_Cast : ATBState<ATBEnemy>
    {
        // Call upon entering given state
        public override void OnEnter()
        {
            Owner.StartCoroutine(CastAndExit());
            Owner.GetComponent<Animator>().SetTrigger("Cast");
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
            yield return SpellManager.instance.Cast(Owner.Caster.Spell, Owner.Caster, Owner.Caster.TargetPos);
            Source.PerformTransition(ATBStateID.AfterCast);
        }

    }
}
