using System.Collections;
using System.Linq;
using UnityEngine;

namespace ATB3
{
    public class ATBStateEnemy_Cast : ATBState<ATBEnemy>
    {
        // The ID for this specific ATBState
        public override ATBStateID StateID { get { return ATBStateID.Cast; } }

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
            var AI = Owner.GetComponent<CasterAI>();
            yield return SpellManager.instance.Cast(Owner.Caster.Spell, Owner.Caster, Owner.Caster.TargetPos);
            AI.OnAfterCast?.Invoke();
            Source.PerformTransition(ATBTransition.ToAfterCast);
        }

    }
}
