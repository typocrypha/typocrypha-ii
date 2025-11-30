using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections.Generic;

public class MoveEventHandler : MonoBehaviour, IMoveHandler
{
    public Dictionary<MoveDirection, bool> DirectionalTriggers { get; private set; } = new Dictionary<MoveDirection, bool>();
    public UnityEvent Response = new UnityEvent();

    public MoveEventHandler EnableTriggers(params MoveDirection[] directions)
    {
        foreach (var d in directions) DirectionalTriggers[d] = true;
        return this;
    }

    public MoveEventHandler DisableTriggers(params MoveDirection[] directions)
    {
        foreach (var d in directions) DirectionalTriggers[d] = false;
        return this;
    }

    public MoveEventHandler AddListeners(params UnityAction[] actions)
    {
        foreach (var act in actions) Response.AddListener(act);
        return this;
    }

    public void OnMove(AxisEventData eventData)
    {
        foreach (var trigger in DirectionalTriggers)
            if (eventData.moveDir == trigger.Key && trigger.Value) Response.Invoke();
    }
}
