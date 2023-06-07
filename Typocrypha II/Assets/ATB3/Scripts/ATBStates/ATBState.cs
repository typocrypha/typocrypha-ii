using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
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
        // General States ∇ 
        Dead,
        Fled,
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
        CastDisabled,
        AllyMenu,
    }

    //================================================================//
    // STATE
    // States with events to be processed by the State Machine.
    //================================================================//

    [System.Serializable]
    public abstract class ATBState<T> where T : ATBActor
    {
        public ATBStateID StateID { get; set; } = ATBStateID.NullATBStateID; 
        private T owner; 
        public T Owner { get { return owner; } set { owner = value; } }
        public ATBStateMachine<T> Source { get; set; }

        // Constructor for ATBState
        // (assigns an ATBActor as an owner for the State to run functions)
        public ATBState()
        {
            owner = null;
            Source = null;
        }
        public ATBState(T actor, ATBStateMachine<T> machine)
        {
            owner = actor;
            Source = machine;
        }

        // set owner for the ATBState
        public void SetOwner(T actor)
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
