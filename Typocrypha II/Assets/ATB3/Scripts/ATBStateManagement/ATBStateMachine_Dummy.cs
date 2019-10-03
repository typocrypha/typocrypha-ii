using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    //================================================================//
    // TEST STATE MACHINE
    // Checking if state machine code works
    //================================================================//

    public class ATBStateMachine_Dummy : ATBStateMachine<ATBEnemy>
    {

        //----------------------------------------------------------------//
        // STATE MACHINE FUNCTIONS                                        //
        //----------------------------------------------------------------//

        // Appends the machine's states to the given state list (should be called on awake)
        // Add your states here!
        protected override void InitializeStates()
        {
            AddState(new ATBStateEnemy_Charge() { Owner = Owner, Source = this }, ATBStateID.Charge);
            AddState(new ATBStateEnemy_Stunned() { Owner = Owner, Source = this }, ATBStateID.Stunned);
        }
    }
}