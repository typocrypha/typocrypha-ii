using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

        public bool HasReadyAllies => Battlefield.instance.Actors.Any(a => a is ATBAlly ally && (ally.allyMenu?.CanCast ?? false));

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


        // Is the ATB system currently in Solo mode?
        public bool ProcessingActions => actionQueue.Count != 0;
        /// <summary>
        /// The current SoloActor. If the ATB system is not in solo, returns null
        /// </summary>
        public ATBActor SoloActor => ProcessingActions ? actionQueue.Peek().Actor : null;
        // Stack for managing when actors have solo activity (casting)
        private readonly Queue<ATBAction> actionQueue = new Queue<ATBAction>();

        // Enter solo mode for this actor
        public void QueueSolo(ATBAction action)
        {
            actionQueue.Enqueue(action);
            if (action.Actor is ATBPlayer && !Typocrypha.Keyboard.instance.PH.Pause)
            {
                Typocrypha.Keyboard.instance.PH.Pause = true;
            }
            if (actionQueue.Count == 1)
            {
                BattleManager.instance.SetBattleEventPause(true); // Pause Battle events.
                Battlefield.instance.PH.Pause = true; // Pause battle field
                DoSolo(action);
            }
        }

        private void DoSolo(ATBAction action)
        {
            if (action.IsValid)
            {
                action.Actor.Pause = false; // TODO: is this needed?
                StartCoroutine(DoSoloCR(action));
            }
            else
            {
                ExitSolo(action);
            }
        }

        private IEnumerator DoSoloCR(ATBAction action)
        {
            var routine = action.Action.Invoke();
            if(routine != null)
            {
                yield return routine;
            }
            ExitSolo(action);
        }

        // Exit solo mode for this actor (should be at top of stack)
        private void ExitSolo(ATBAction action)
        {
            if (actionQueue.Count == 0)
            {
                Debug.LogWarning($"ExitSolo with empty solo queue:{action}");
                return;
            }
            //Debug.Log("exit:" + soloActor.gameObject.name);
            if (action != actionQueue.Dequeue())
                Debug.LogError("StateManager: solo queue Mismatch");
            if (action.Actor is ATBPlayer)
            {
                Typocrypha.Keyboard.instance.PH.Pause = false;
            }
            action.OnComplete?.Invoke();
            // If queue is now empty, unpause all actors
            if (actionQueue.Count == 0)
            {
                foreach (ATBActor actor in Battlefield.instance.Actors)
                    actor.isCast = false;
                BattleManager.instance.SetBattleEventPause(false);
                Battlefield.instance.PH.Pause = false; // unpause battle field
            }
            // Otherwise, give solo to next in queue
            else
            {
                action.Actor.Pause = true;
                DoSolo(actionQueue.Peek());
            }
        }

        public class ATBAction
        {
            public System.Func<Coroutine> Action { get; set; }
            public System.Action OnComplete { get; set; }
            public ATBActor Actor { get; set; }
            public bool IsValid
            {
                get
                {
                    if (Action == null)
                        return false;
                    //if (Actor is Caster caster && caster.IsDeadOrFled) // TODO, unify systems
                    //    return false;
                    return true;
                }
            }
        }
    }
}

