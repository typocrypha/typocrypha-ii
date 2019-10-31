using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    //================================================================//
    // STATE MACHINE
    // Machine contained in all Actors to manage individual states.
    //================================================================//
    // Frankensteined from this example:
    // http://wiki.unity3d.com/index.php/Finite_State_Machine
    // A Finite State Machine System based on Chapter 3.1 of Game Programming Gems 1 by Eric Dybsand
    // Written by Roberto Cezar Bianchini, July 2010
    // ===============================================================//
    [RequireComponent(typeof(ATBActor))]
    [DisallowMultipleComponent]
    public abstract class ATBStateMachine<T> : MonoBehaviour, IPausable, IATBStateMachine where T : ATBActor
    {
        #region IPausable
        PauseHandle ph;
        public PauseHandle PH
        {
            get => ph;
        }

        public void OnPause(bool b)
        {
            enabled = !b;
        }
        #endregion
        //----------------------------------------------------------------//
        // PROPERTIES                                                     //
        //----------------------------------------------------------------//

        // ATBActor that owns this state machine
        protected T Owner { get; private set; }

        // map of all the machine's states linked to an ID
        // Used for transitioning!
        protected Dictionary<ATBStateID, ATBState<T>> states = new Dictionary<ATBStateID, ATBState<T>>();

        // The only way one can change the state of the machine is by performing a transition
        // Don't change the CurrentState directly (unless initializing with SetState())
        public ATBStateID CurrentStateID { get; set; }
        public ATBState<T> CurrentState { get; private set; }
        public ATBStateID PreviousStateID { get; private set; }
        public ATBState<T> PreviousState { get; private set; }

        //----------------------------------------------------------------//
        // RUNTIME FUNCTIONS                                              //
        //----------------------------------------------------------------//

        void Awake()
        {
            ph = new PauseHandle(OnPause);
            Owner = GetComponent<T>();
            InitializeStates();
            foreach (var kvp in states)
                kvp.Value.SetOwner(Owner);
        }

        void Start()
        {
            CurrentState.OnEnter();
        }

        void FixedUpdate()
        {
            CurrentState.OnUpdate();
        }

        //----------------------------------------------------------------//
        // STATE MACHINE FUNCTIONS                                        //
        //----------------------------------------------------------------//

        // Appends the machine's states to the given state list (should be called on awake)
        // Add your states here!
        protected abstract void InitializeStates();

        /// <summary>
        /// This method places new states inside the FSM,
        /// or prints an ERROR message if the state was already inside the List.
        /// First state added is also the initial state.
        /// </summary>
        public void AddState(ATBState<T> s, ATBStateID id)
        {
            // Check for Null reference before deleting
            if (s == null)
            {
                Debug.LogError("FSM ERROR: Null reference is not allowed");
                return;
            }

            if(states.ContainsKey(id))
            {
                Debug.LogError("FSM ERROR: Duplicate state ID registration");
                return;
            }

            // First State inserted is also the Initial state,
            // the state the machine is in when the simulation begins
            if (states.Count == 0)
            {
                states.Add(id, s);
                s.StateID = id;
                SetState(s);
                return;
            }

            states.Add(id, s);
            s.StateID = id;
        }

        /// <summary>
        /// This method tries to change the state the FSM is in based on
        /// the current state and the transition passed. If current state
        ///  doesn't have a target state for the transition passed, 
        /// an ERROR message is printed.
        /// </summary>
        public void PerformTransition(ATBStateID transitionTo)
        {
            // Check for NullTransition before changing the current state
            if (transitionTo == ATBStateID.NullATBStateID)
            {
                Debug.LogError("FSM ERROR: NullStateID is not allowed for a real transition");
                return;
            }
            if(!states.ContainsKey(transitionTo))
            {
                Debug.LogError("FSM ERROR: " + name + " does not have a state with the ID: " + transitionTo.ToString());
                return;
            }

            // SPECIAL CASE: If we're returning to a previous state, rollback
            if (transitionTo == ATBStateID.PreviousState)
            {
                var tempState = PreviousState;
                PreviousStateID = CurrentState.StateID;
                CurrentStateID = tempState.StateID;
                // Do the post processing of the state before setting the new one
                CurrentState.OnExit();
                PreviousState = CurrentState;
                CurrentState = tempState;
                // Reset the state to its desired condition before it can reason or act
                CurrentState.OnEnter();
            }
            else
            {
                // Update the currentStateID and currentState		
                PreviousStateID = CurrentStateID;
                CurrentStateID = transitionTo;
                // Do the post processing of the state before setting the new one
                CurrentState.OnExit();
                PreviousState = CurrentState;
                CurrentState = states[transitionTo];
                // Reset the state to its desired condition before it can reason or act
                CurrentState.OnEnter();
            }
        }

        // force set the current ATB state for the machine (i.e. change initialization)
        // this does not use transitions or activate OnEnter/Exit
        private void SetState(ATBState<T> state)
        {
            PreviousState = CurrentState;
            PreviousStateID = CurrentStateID;
            CurrentState = state;
            CurrentStateID = state.StateID;
        }
    }
}
