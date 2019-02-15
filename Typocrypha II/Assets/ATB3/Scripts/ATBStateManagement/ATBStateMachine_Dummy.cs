using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    //================================================================//
    // TEST STATE MACHINE
    // Checking if state machine code works
    //================================================================//

    public class ATBStateMachine_Dummy : ATBStateMachine
    {
        //----------------------------------------------------------------//
        // PROPERTIES                                                     //
        //----------------------------------------------------------------//

        public ATBEnemy Owner { get => owner as ATBEnemy; }

        //----------------------------------------------------------------//
        // STATE MACHINE FUNCTIONS                                        //
        //----------------------------------------------------------------//

        // Appends the machine's states to the given state list (should be called on awake)
        // Add your states here!
        protected override void InitializeStates()
        {
            AddState(new ATBStateEnemy_Charge() { Owner = this.Owner, Source = this });
            AddState(new ATBStateEnemy_Stunned() { Owner = this.Owner, Source = this });
        }

        // Appends the machine's transitions to the given transition map (should be called on awake)
        // Add your transitions here!
        // transitionMap says: trans rights!
        protected override void InitializeTransitions()
        {
            AddTransition(ATBTransition.ToStun, ATBStateID.Stunned);
            AddTransition(ATBTransition.ExitStun, ATBStateID.Charge);
            AddTransition(ATBTransition.ToPreCast, ATBStateID.Stunned);
        }

    }
}