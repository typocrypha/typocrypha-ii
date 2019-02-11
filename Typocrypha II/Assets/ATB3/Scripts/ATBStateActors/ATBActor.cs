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

    public abstract class ATBActor : MonoBehaviour
    {
        //----------------------------------------------------------------//
        // PROPERTIES                                                     //
        //----------------------------------------------------------------//

        private bool _paused; // Is the actor paused?
        public ATBStateMachine stateMachine; // State machine for this actor

        public bool pause // Is this actor paused or not?
        {
            get
            {
                return _paused;
            }
            set
            {
                _paused = value ? false : true;
            }
        }
        public bool isCast; // Is the actor in cast sequence? Isn't unset until all chains are finished.

        //----------------------------------------------------------------//
        // GENERIC ACTOR FUNCTIONS                                        //
        //----------------------------------------------------------------//

        // Call to do initial setup on actor
        public abstract void Setup();

        // Checks if the current state name matches given string
        public bool isCurrentState(string stateName)
        {
            //return currStateHash == Animator.StringToHash(stateName);
            return false;
        }
    }
}

