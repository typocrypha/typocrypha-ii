using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    //================================================================//
    // STATE MACHINE
    // Machine contained in all Actors to manage individual states.
    //================================================================//

    public abstract class ATBStateMachine : MonoBehaviour
    {
        //----------------------------------------------------------------//
        // PROPERTIES                                                     //
        //----------------------------------------------------------------//

        // map of all the machine's transitions and their end point states
        // (override and define this in individual child state machines)
        private Dictionary<ATBTransition, ATBStateID> transitionMap = new Dictionary<ATBTransition, ATBStateID>();
        // list of all the states allowed in this machine 
        // (because you have to call [new ATBStateBlah()] for each state, add to list on awake())
        private List<ATBState> states;

        // The only way one can change the state of the machine is by performing a transition
        // Don't change the CurrentState directly (unless initializing with SetState())
        private ATBStateID currentStateID;
        public ATBStateID CurrentStateID { get { return currentStateID; } }
        private ATBState currentState;
        public ATBState CurrentState { get { return currentState; } }
        private ATBStateID previousStateID;
        public ATBStateID PreviousStateID { get { return previousStateID; } }
        private ATBState previousState;
        public ATBState PreviousState { get { return previousState; } }

        //----------------------------------------------------------------//
        // RUNTIME FUNCTIONS                                              //
        //----------------------------------------------------------------//

        void Awake()
        {
            InitializeStates(states);
            currentState.OnEnter();
        }

        void FixedUpdate()
        {
            currentState.OnUpdate();
        }

        //----------------------------------------------------------------//
        // STATE MACHINE FUNCTIONS                                        //
        //----------------------------------------------------------------//

        // Appends the machine's states to the given state list (should be called on awake)
        // Add your states here!
        protected abstract void InitializeStates(List<ATBState> stateList);

        /// <summary>
        /// This method places new states inside the FSM,
        /// or prints an ERROR message if the state was already inside the List.
        /// First state added is also the initial state.
        /// </summary>
        public void AddState(ATBState s)
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
                currentState = s;
                currentStateID = s.ID;
                return;
            }

            // Add the state to the List if it's not inside it
            foreach (ATBState state in states)
            {
                if (state.ID == s.ID)
                {
                    Debug.LogError("FSM ERROR: Impossible to add state " + s.ID.ToString() +
                                   " because state has already been added");
                    return;
                }
            }
            states.Add(s);
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
                ATBState tempState = previousState;
                previousStateID = currentState.ID;
                currentStateID = tempState.ID;

                // Do the post processing of the state before setting the new one
                currentState.OnExit();

                previousState = currentState;
                currentState = tempState;

                // Reset the state to its desired condition before it can reason or act
                currentState.OnEnter();
                return;
            }
            else if (id == ATBStateID.NullATBStateID)
            {
                Debug.LogError("FSM ERROR: State " + currentStateID.ToString() + " does not have a target state " +
                               " for transition " + trans.ToString());
                return;
            }
            else
            {
                // Update the currentStateID and currentState		
                previousStateID = currentStateID;
                currentStateID = id;
                foreach (ATBState state in states)
                {
                    if (state.ID == currentStateID)
                    {
                        // Do the post processing of the state before setting the new one
                        currentState.OnExit();

                        previousState = currentState;
                        currentState = state;

                        // Reset the state to its desired condition before it can reason or act
                        currentState.OnEnter();
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
        private void SetState(ATBState state)
        {
            if (states.Contains(state))
            {
                previousState = currentState;
                previousStateID = currentStateID;
                currentState = state;
                currentStateID = state.ID;
            }
            return;
        }
    }
}
