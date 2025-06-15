using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class MoveEventHandler : MonoBehaviour, IMoveHandler
{
    public MoveDirection[] DirectionalTriggers { get; private set; } = default;
    public UnityEvent Response = new UnityEvent();

    public MoveEventHandler SetTrigger(params MoveDirection[] directions)
    {
        DirectionalTriggers = directions;
        return this;
    }

    public MoveEventHandler SetResponse(params UnityAction[] actions)
    {
        foreach (var act in actions) Response.AddListener(act);
        return this;
    }

    public void OnMove(AxisEventData eventData)
    {
        foreach (var trigger in DirectionalTriggers)
          if (eventData.moveDir == trigger) Response.Invoke();
    }
}
