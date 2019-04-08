using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ATB3
{

    //================================================================//
    // ACTOR
    // A generic entity with states in the ATB system.
    //================================================================//

    public abstract class ATBActor : MonoBehaviour, IPausable
    {
        //----------------------------------------------------------------//
        // PROPERTIES                                                     //
        //----------------------------------------------------------------//

        public ATBStateMachine StateMachine; // State machine for this actor
        public string actorName; // Name of actor (debug)

        #region IPausable
        PauseHandle ph;
        public PauseHandle PH
        {
            get => ph;
        }

        public void OnPause(bool b) { }
        #endregion

        public bool pause // Is this actor paused or not?
        {
            get
            {
                return PH.Pause;
            }
            set
            {
                PH.Pause = value;
            }
        }
        public bool isCast; // Is the actor in cast sequence? Isn't unset until all chains are finished.

        //----------------------------------------------------------------//
        // GENERIC ACTOR FUNCTIONS                                        //
        //----------------------------------------------------------------//

        void Awake()
        {
            ph = new PauseHandle(OnPause);
        }

        // Call to do initial setup on actor
        public abstract void Setup();

        // Checks if the current state name matches given string
        public bool isCurrentState(ATBStateID stateID)
        {
            //return currStateHash == Animator.StringToHash(stateName);
            return StateMachine.CurrentStateID == stateID;
        }
    }
}

