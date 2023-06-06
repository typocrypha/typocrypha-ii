using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    //================================================================//
    // PLAYER STATE MACHINE
    // Machine for the player to manage individual states.
    //================================================================//

    public class ATBStateMachine_Player : ATBStateMachine<ATBPlayer>
    {
        //----------------------------------------------------------------//
        // PROPERTIES                                                     //
        //----------------------------------------------------------------//

        //----------------------------------------------------------------//
        // STATE MACHINE FUNCTIONS                                        //
        //----------------------------------------------------------------//

        // Appends the machine's states to the given state list (should be called on awake)
        // Add your states here!
        protected override void InitializeStates()
        {
            AddState(new ATBStatePlayer_Idle() { Owner = Owner, Source = this }, ATBStateID.Idle);
            AddState(new ATBStatePlayer_Cast() { Owner = Owner, Source = this }, ATBStateID.Cast);
        }
    }
}