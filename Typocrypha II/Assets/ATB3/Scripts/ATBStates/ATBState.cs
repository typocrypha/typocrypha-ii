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
        Run,
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
    public abstract class ATBState<T> : IATBState where T : ATBActor
    {
        public ATBStateID StateID { get; set; } = ATBStateID.NullATBStateID; 
        public T Owner { get; set; }
        public ATBStateMachine<T> Source { get; set; }

        // Constructor for ATBState
        // (assigns an ATBActor as an owner for the State to run functions)
        public ATBState()
        {
            Owner = null;
            Source = null;
        }
        public ATBState(T actor, ATBStateMachine<T> machine)
        {
            Owner = actor;
            Source = machine;
        }

        // set owner for the ATBState
        public void SetOwner(T actor)
        {
            Owner = actor;
        }

        protected void SetAnimation(int trigger)
        {
            Source.SetAnimation(trigger);
        }

        protected bool CheckDeathOrRun(Caster caster)
        {
            if (caster.BStatus == Caster.BattleStatus.Dead)
            {
                Source.PerformTransition(ATBStateID.Dead);
                return true;
            }
            if (caster.BStatus == Caster.BattleStatus.Fled)
            {
                Source.PerformTransition(ATBStateID.Run);
                return true;
            }
            return false;
        }

        // Call upon entering given state
        public abstract void OnEnter();

        // Call on fixed update while in given state
        public abstract void OnUpdate();

        // Call upon exiting given state
        public abstract void OnExit();
    }
}
