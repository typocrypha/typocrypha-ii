using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    //================================================================//
    // ENEMY STATE MACHINE
    // Machine contained in all Enemies to manage individual states.
    //================================================================//

    public partial class ATBStateMachine_Enemy : ATBStateMachine
    {
        //----------------------------------------------------------------//
        // PROPERTIES                                                     //
        //----------------------------------------------------------------//

        public new ATBEnemy owner;
        
        // map of all the machine's transitions and their end point states
        // (override and define this in individual child state machines)
        private Dictionary<ATBTransition, ATBStateID> transitionMap = new Dictionary<ATBTransition, ATBStateID>() {
            {ATBTransition.Stun, ATBStateID.Stunned},           // If stunned, go into stunned state
            {ATBTransition.ExitStun, ATBStateID.Charge},        // If exiting stun, go into charge state
            {ATBTransition.ChargeFinish, ATBStateID.PreCast},   // If finished charge, go into precast state
            {ATBTransition.CastStart, ATBStateID.BeforeCast},   // If about to cast, go into beforecast state
            {ATBTransition.CastPerform, ATBStateID.Cast},       // If finishing beforecast, go into cast state
            {ATBTransition.CastEnd, ATBStateID.AfterCast},      // If performed spell, go into aftercast state
            {ATBTransition.ChargeStart, ATBStateID.Charge}      // If ready to charge spell again, go into charge state
        };

        //----------------------------------------------------------------//
        // STATE MACHINE FUNCTIONS                                        //
        //----------------------------------------------------------------//

        // Appends the machine's states to the given state list (should be called on awake)
        // Add your states here!
        protected override void InitializeStates()
        {
            AddState(new ATBStateEnemy_Charge(this.owner, this));
            AddState(new ATBStateEnemy_PreCast(this.owner, this));
            AddState(new ATBStateEnemy_BeforeCast(this.owner, this));
            AddState(new ATBStateEnemy_Cast(this.owner, this));
            AddState(new ATBStateEnemy_AfterCast(this.owner, this));
            AddState(new ATBStateEnemy_Stunned(this.owner, this));
        }

    }
}