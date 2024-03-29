﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB3
{
    //================================================================//
    // STATE MANAGER
    // Manages state machine events; 
    // also contains all events that can be sent.
    //================================================================//

    public partial class ATBManager : MonoBehaviour
    {
        public static ATBManager instance;
        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
            }
        }

        //----------------------------------------------------------------//
        // EVENT DATA                                                     //
        //----------------------------------------------------------------//

        // Is the ATB system currently in Solo mode?
        public bool InSolo => soloStack.Count != 0;
        /// <summary>
        /// The current SoloActor. If the ATB system is not in solo, returns null
        /// </summary>
        public ATBActor SoloActor => InSolo ? soloStack.Peek() : null;
        // Events sent
        // static List<StateEventObj> eventQueue = new List<StateEventObj>(); 
        static List<string> eventQueue = new List<string>();
        // Stack for managing when actors have solo activity (casting)
        private readonly Stack<ATBActor> soloStack = new Stack<ATBActor>();

        //----------------------------------------------------------------//
        // STATE MANAGEMENT                                               //
        //----------------------------------------------------------------//

        // Check queue for new messages
        void Update()
        {
            //processQueue();
        }

        // Process queue (FIFO: one event per update frame)
        void ProcessQueue()
        {
            //if (eventQueue.Count == 0) return;
            //StateEventObj obj = eventQueue[0];
            //this.SendMessage(obj.stateEvent, obj.args);
            //eventQueue.Remove(obj);
        }

        // Send an event to the manager to put in the queue
        //public static void sendEvent(string stateEvent, StateEventArgs args)
        //{
        //    eventQueue.Add(new StateEventObj(stateEvent, args));
        //}

        // Send an event to the manager to put in the queue
        //public static void sendEvent(string stateEvent, Actor actor, int hashID)
        //{
        //    //Debug.Log("Send event:" + actor.gameObject.name + ":" + stateEvent);
        //    sendEvent(stateEvent, new StateEventArgs(actor, hashID));
        //}

        // Set the pause value of all actors
        void SetPauseAll(bool value)
        {
            foreach (ATBActor actor in Battlefield.instance.Actors)
                actor.Pause = value;
        }

        // Enter solo mode for this actor
        public void EnterSolo(ATBActor soloActor)
        {
            //Debug.Log("enter:" + soloActor.gameObject.name);

            if (soloStack.Count == 0)
            {
                if (BattleManager.instance != null)
                    BattleManager.instance.PH.Pause = true; // Pause Battle events.
                Battlefield.instance.PH.Pause = true; // Pause battle field
            }
            else
                soloStack.Peek().Pause = true;
            soloActor.Pause = false;
            soloStack.Push(soloActor);
        }

        // Exit solo mode for this actor (should be at top of stack)
        public void ExitSolo(ATBActor soloActor)
        {
            //Debug.Log("exit:" + soloActor.gameObject.name);
            if (soloActor != soloStack.Pop())
                Debug.LogError("StateManager: Solo Stack Mismatch");
            // If stack is now empty, unpause all actors
            if (soloStack.Count == 0)
            {
                //CastBar.MainBar.focus = true;
                foreach (ATBActor actor in Battlefield.instance.Actors)
                    actor.isCast = false;
                if (BattleManager.instance != null)
                    BattleManager.instance.PH.Pause = false; // Unpause Battle events.
                Battlefield.instance.PH.Pause = false; // Pause battle field
            }
            // Otherwise, give solo to next in stack
            else
            {
                soloActor.Pause = true;
                soloStack.Peek().Pause = false;
            }
        }

        //----------------------------------------------------------------//
        // EVENTS TO BE SENT                                              //
        //----------------------------------------------------------------//

        // Print a debug message
        //public void ping(StateEventArgs args)
        //{
        //    Debug.Log("Ping:" + args);
        //}

        // Allow the state to continue
        // Sent when a state is ready to exit
        //public void stateContinue(StateEventArgs args)
        //{
        //    args.actor.stateMachine.SetTrigger("Continue");
        //}

        // Save the progress of the current state (will resume from that point next time)
        //public void saveProgress(StateEventArgs args)
        //{
        //    args.actor.stateMachine.SetBool("SaveProgress", true);
        //}

        // Pause the actor who sent this event
        //public void pause(StateEventArgs args)
        //{
        //    args.actor.pause = true;
        //}

        // Unpause the actor who sent this event
        //public void unpause(StateEventArgs args)
        //{
        //    args.actor.pause = false;
        //}

        // Stun the actor who sent this event
        //public void stun(StateEventArgs args)
        //{
        //    args.actor.stateMachine.Play("Stunned");
        //}
    }
}

