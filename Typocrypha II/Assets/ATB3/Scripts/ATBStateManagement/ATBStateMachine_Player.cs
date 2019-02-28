using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    //================================================================//
    // PLAYER STATE MACHINE
    // Machine for the player to manage individual states.
    //================================================================//

    public class ATBStateMachine_Player : ATBStateMachine
    {
        //----------------------------------------------------------------//
        // PROPERTIES                                                     //
        //----------------------------------------------------------------//

        public ATBPlayer Owner { get => owner as ATBPlayer; }

        //----------------------------------------------------------------//
        // STATE MACHINE FUNCTIONS                                        //
        //----------------------------------------------------------------//

        // Appends the machine's states to the given state list (should be called on awake)
        // Add your states here!
        protected override void InitializeStates()
        {
            AddState(new ATBStatePlayer_Idle() { Owner = this.Owner, Source = this });
            AddState(new ATBStatePlayer_BeforeCast() { Owner = this.Owner, Source = this });
            AddState(new ATBStatePlayer_Cast() { Owner = this.Owner, Source = this });
            AddState(new ATBStatePlayer_AfterCast() { Owner = this.Owner, Source = this });
        }

        // Appends the machine's transitions to the given transition map (should be called on awake)
        // Add your transitions here!
        // transitionMap says: trans rights!
        protected override void InitializeTransitions()
        {
            AddTransition(ATBTransition.ToBeforeCast, ATBStateID.BeforeCast);    // If about to cast, go into beforecast state
            AddTransition(ATBTransition.ToCast, ATBStateID.Cast);                // If finishing beforecast, go into cast state
            AddTransition(ATBTransition.ToAfterCast, ATBStateID.AfterCast);      // If performed spell, go into aftercast state
            AddTransition(ATBTransition.ToIdle, ATBStateID.Idle);                // If ready to type again, go into idle state
        }
    }
}