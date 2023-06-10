using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    //================================================================//
    // ENEMY STATE MACHINE
    // Machine contained in all Enemies to manage individual states.
    //================================================================//

    public class ATBStateMachine_Enemy : ATBStateMachine<ATBEnemy>
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
            AddState(new ATBStateEnemy_Charge() { Owner = Owner, Source = this }, ATBStateID.Charge);
            AddState(new ATBStateEnemy_PreCast() { Owner = Owner, Source = this }, ATBStateID.PreCast);
            AddState(new ATBStateEnemy_Cast() { Owner = Owner, Source = this }, ATBStateID.Cast);
            AddState(new ATBStateEnemy_Stunned() { Owner = Owner, Source = this }, ATBStateID.Stunned);
            AddState(new ATBStateEnemy_Dead() { Owner = Owner, Source = this }, ATBStateID.Dead);
            AddState(new ATBStateEnemy_Fled() { Owner = Owner, Source = this }, ATBStateID.Fled);
        }
    }
}