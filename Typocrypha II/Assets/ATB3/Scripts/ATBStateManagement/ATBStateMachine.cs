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

        // map of all the machine's transitions and their end point states
        // (override and define this in individual child state machines)
        protected Dictionary<ATBTransition, ATBStateID> transitionMap = new Dictionary<ATBTransition, ATBStateID>();
        // list of all the states allowed in this machine 
        // (because you have to call [new ATBStateBlah()] for each state, add to list on awake())
        private List<ATBState<T>> states = new List<ATBState<T>>();

        // The only way one can change the state of the machine is by performing a transition
        // Don't change the CurrentState directly (unless initializing with SetState())
        public ATBStateID CurrentStateID { get; set; }
        public ATBState<T> CurrentState { get; private set; }
        public ATBStateID PreviousStateID { get; set; }
        public ATBState<T> PreviousState { get; private set; }

        //----------------------------------------------------------------//
        // RUNTIME FUNCTIONS                                              //
        //----------------------------------------------------------------//

        void Awake()
        {
            ph = new PauseHandle(OnPause);
            Owner = GetComponent<T>();
            InitializeStates();
            InitializeTransitions();
            foreach (var state in states)
            {
                state.SetOwner(Owner);
            }
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
        protected abstract void InitializeTransitions();

        /// <summary>
        /// This method places new states inside the FSM,
        /// or prints an ERROR message if the state was already inside the List.
        /// First state added is also the initial state.
        /// </summary>
        public void AddState(ATBState<T> s)
        {
            // Check for Null reference before deleting
            if (s == null)
            {
                Debug.LogError("FSM ERROR: Null reference is not allowed");
            }

            // First State inserted is also the Initial state,
            //   the state the machine is in when the simulation begins
            if (states.Count == 0)
            {
                states.Add(s);
                CurrentState = s;
                CurrentStateID = s.StateID;
                //Debug.Log(owner.actorName + " intial current state is " + currentStateID.ToString());
                return;
            }

            // Add the state to the List if it's not inside it
            foreach (var state in states)
            {
                if (state.StateID == s.StateID)
                {
                    Debug.LogError("FSM ERROR: Impossible to add state " + s.StateID.ToString() +
                                   " because state has already been added");
                    return;
                }
            }
            states.Add(s);
        }

        public void AddTransition(ATBTransition trans, ATBStateID id)
        {
            // Check if anyone of the args is invalid
            if (trans == ATBTransition.NullATBTransition)
            {
                Debug.LogError("FSMState ERROR: NullTransition is not allowed for a real transition");
                return;
            }

            if (id == ATBStateID.NullATBStateID)
            {
                Debug.LogError("FSMState ERROR: NullStateID is not allowed for a real ID");
                return;
            }

            transitionMap.Add(trans, id);
        }

        /// <summary>
        /// This method tries to change the state the FSM is in based on
        /// the current state and the transition passed. If current state
        ///  doesn't have a target state for the transition passed, 
        /// an ERROR message is printed.
        /// </summary>
        public void PerformTransition(ATBTransition trans)
        {
            // Check for NullTransition before changing the current state
            if (trans == ATBTransition.NullATBTransition)
            {
                Debug.LogError("FSM ERROR: NullTransition is not allowed for a real transition");
                return;
            }

            // Check if the currentState has the transition passed as argument
            ATBStateID id = GetOutputState(trans);

            // SPECIAL CASE: If we're returning to a previous state, rollback
            if (id == ATBStateID.PreviousState)
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
                return;
            }
            else if (id == ATBStateID.NullATBStateID)
            {
                Debug.LogError("FSM ERROR: State " + CurrentStateID.ToString() + " does not have a target state " +
                               " for transition " + trans.ToString());
                return;
            }
            else
            {
                // Update the currentStateID and currentState		
                PreviousStateID = CurrentStateID;
                CurrentStateID = id;
                foreach (var state in states)
                {
                    if (state.StateID == CurrentStateID)
                    {
                        // Do the post processing of the state before setting the new one
                        CurrentState.OnExit();

                        PreviousState = CurrentState;
                        CurrentState = state;

                        // Reset the state to its desired condition before it can reason or act
                        CurrentState.OnEnter();
                        break;
                    }
                }
            }
            return;
        }

        /// <summary>
        /// This method returns the new state the FSM should be if
        ///    this state receives a transition and 
        /// </summary>
        public ATBStateID GetOutputState(ATBTransition trans)
        {
            // Check if the map has this transition
            if (transitionMap.ContainsKey(trans))
            {
                return transitionMap[trans];
            }
            return ATBStateID.NullATBStateID;
        }

        // force set the current ATB state for the machine (i.e. change initialization)
        // this does not use transitions or activate OnEnter/Exit
        private void SetState(ATBState<T> state)
        {
            if (states.Contains(state))
            {
                PreviousState = CurrentState;
                PreviousStateID = CurrentStateID;
                CurrentState = state;
                CurrentStateID = state.StateID;
            }
            return;
        }
    }
}
