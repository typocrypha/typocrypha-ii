﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    //================================================================//
    // ENEMY STATE MACHINE
    // Machine contained in all Enemies to manage individual states.
    //================================================================//

    public class ATBStateMachine_Enemy : ATBStateMachine
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
            AddState(new ATBStateEnemy_PreCast() { Owner = this.Owner, Source = this });
            AddState(new ATBStateEnemy_BeforeCast() { Owner = this.Owner, Source = this });
            AddState(new ATBStateEnemy_Cast() { Owner = this.Owner, Source = this });
            AddState(new ATBStateEnemy_AfterCast() { Owner = this.Owner, Source = this });
            AddState(new ATBStateEnemy_Stunned() { Owner = this.Owner, Source = this });
        }

        // Appends the machine's transitions to the given transition map (should be called on awake)
        // Add your transitions here!
        // transitionMap says: trans rights!
        protected override void InitializeTransitions()
        {
            AddTransition(ATBTransition.ToStun, ATBStateID.Stunned);             // If stunned, go into stunned state
            AddTransition(ATBTransition.ExitStun, ATBStateID.Charge);            // If exiting stun, go into charge state
            AddTransition(ATBTransition.ToPreCast, ATBStateID.PreCast);          // If finished charge, go into precast state
            AddTransition(ATBTransition.ToBeforeCast, ATBStateID.BeforeCast);    // If about to cast, go into beforecast state
            AddTransition(ATBTransition.ToCast, ATBStateID.Cast);                // If finishing beforecast, go into cast state
            AddTransition(ATBTransition.ToAfterCast, ATBStateID.AfterCast);      // If performed spell, go into aftercast state
            AddTransition(ATBTransition.ToCharge, ATBStateID.Charge);             // If ready to charge spell again, go into charge state
        }
    }
}