using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    //================================================================//
    // ALLY STATE MACHINE
    // Machine contained in all allies to manage individual states.
    //================================================================//

    public class ATBStateMachine_Ally : ATBStateMachine<ATBAlly>
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
            AddState(new ATBStateAlly_Charge() { Owner = Owner, Source = this });
            AddState(new ATBStateAlly_BeforeCast() { Owner = Owner, Source = this });
            AddState(new ATBStateAlly_Cast() { Owner = Owner, Source = this });
            AddState(new ATBStateAlly_AfterCast() { Owner = Owner, Source = this });
            AddState(new ATBStateAlly_CastDisabled() { Owner = Owner, Source = this });
            AddState(new ATBStateAlly_Stunned() { Owner = Owner, Source = this });
            AddState(new ATBStateAlly_Menu() { Owner = Owner, Source = this });
        }

        // Appends the machine's transitions to the given transition map (should be called on awake)
        // Add your transitions here!
        // transitionMap says: trans rights!
        protected override void InitializeTransitions()
        {
            AddTransition(ATBTransition.ToStun, ATBStateID.Stunned);             // If stunned, go into stunned state
            AddTransition(ATBTransition.ExitStun, ATBStateID.Charge);            // If exiting stun, go into charge state
            AddTransition(ATBTransition.ToBeforeCast, ATBStateID.BeforeCast);    // If about to cast, go into beforecast state
            AddTransition(ATBTransition.ToCast, ATBStateID.Cast);                // If finishing beforecast, go into cast state
            AddTransition(ATBTransition.ToAfterCast, ATBStateID.AfterCast);      // If performed spell, go into aftercast state
            AddTransition(ATBTransition.ToCharge, ATBStateID.Charge);            // If ready to charge spell again, go into charge state
            AddTransition(ATBTransition.ToAllyMenu, ATBStateID.AllyMenu);            // Go to menu
        }
    }
}