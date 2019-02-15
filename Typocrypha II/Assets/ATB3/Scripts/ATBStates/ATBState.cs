using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{

    //================================================================//
    // ATBStateTransition
    // ID enum of events that trigger event changes
    //================================================================//

    /// <summary>
    /// Place the labels for the Transitions in this enum.
    /// Don't change the first label, NullTransition as FSMSystem class uses it.
    /// </summary>
    public enum ATBTransition
    {
        NullATBTransition = 0, // Use this transition to represent a non-existing transition in your system
        ToStun,
        ToPreCast,
        ToBeforeCast,
        ToCast,
        ToAfterCast,
        ToCharge,
        ExitStun,
        ToIdle,
    }

    //================================================================//
    // ATBStateID
    // ID enum of state IDs
    //================================================================//

    /// <summary>
    /// Place the labels for the States in this enum.
    /// Don't change the first label, NullTransition as FSMSystem class uses it.
    /// </summary>
    public enum ATBStateID
    {
        NullATBStateID = 0, // Use this ID to represent a non-existing State in your system
        // Special State for the state machine to rollback into previous states ∇ 
        PreviousState,
        // Player-Specific States ∇ 
        Idle,
        // Non-Player General States ∇ 
        Stunned,
        Charge,
        BeforeCast,
        Cast,
        AfterCast,
        // Enemy-Specific States ∇ 
        PreCast,
        // Ally-Specific States ∇ 
        AllyMenu,
        CastDisabled,
    }

    //================================================================//
    // STATE
    // States with events to be processed by the State Machine.
    //================================================================//

    [System.Serializable]
    public abstract class ATBState
    {
        //protected ATBStateID stateID = 0;
        public virtual ATBStateID StateID { get { return ATBStateID.NullATBStateID; } }
        private ATBActor owner; 
        public ATBActor Owner { get { return owner; } set { owner = value; } }
        private ATBStateMachine source;
        public ATBStateMachine Source { get { return source; } set { source = value;  } }
        public float timePassed = 0.0f;    // Amount of time spent in the state

        // Constructor for ATBState
        // (assigns an ATBActor as an owner for the State to run functions)
        public ATBState()
        {
            owner = null;
            source = null;
        }
        public ATBState(ATBActor actor, ATBStateMachine machine)
        {
            owner = actor;
            source = machine;
        }

        // set owner for the ATBState
        public void SetOwner(ATBActor actor)
        {
            owner = actor;
        }

        // Call upon entering given state
        public abstract void OnEnter();

        // Call on fixed update while in given state
        public abstract void OnUpdate();

        // Call upon exiting given state
        public abstract void OnExit();
    }
}
