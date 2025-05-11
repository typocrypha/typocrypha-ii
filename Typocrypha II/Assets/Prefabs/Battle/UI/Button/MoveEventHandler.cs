using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class MoveEventHandler : MonoBehaviour, IMoveHandler
{
    public MoveDirection[] DirectionalTriggers = new MoveDirection[] { MoveDirection.None };
    public UnityEvent Response;

    public void OnMove(AxisEventData eventData)
    {
        foreach (var trigger in DirectionalTriggers)
          if (eventData.moveDir == trigger) Response.Invoke();
    }
}
