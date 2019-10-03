using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    //================================================================//
    // ALLY STATE MACHINE
    // Machine contained in all allies to manage individual states.
    //================================================================//

    public class ATBStateMachine_AllyAutonomous : ATBStateMachine<ATBAlly>
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
            AddState(new ATBStateAlly_Charge() { Owner = Owner, Source = this }, ATBStateID.Charge);
            AddState(new ATBStateAlly_BeforeCast() { Owner = Owner, Source = this }, ATBStateID.BeforeCast);
            AddState(new ATBStateAlly_Cast() { Owner = Owner, Source = this }, ATBStateID.Cast);
            AddState(new ATBStateAlly_AfterCast() { Owner = Owner, Source = this }, ATBStateID.AfterCast);
            AddState(new ATBStateAlly_CastDisabled() { Owner = Owner, Source = this }, ATBStateID.CastDisabled);
            AddState(new ATBStateAlly_Stunned() { Owner = Owner, Source = this }, ATBStateID.Stunned);
        }
    }
}