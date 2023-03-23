using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetReticle : MonoBehaviour, IPausable
{
    #region IPausable
    public PauseHandle PH { get; private set; }
    // Stop target reticule movement and animation.
    public void OnPause(bool pause)
    {
        enabled = !pause;
        GetComponent<Animator>().speed = pause ? 0f : 1f;
    }
    #endregion
    void Awake()
    {
        PH = new PauseHandle(OnPause);
    }

    void Start()
    {
        PH.SetParent(BattleManager.instance.PH);
        // Initialize position
        lastTargetPos = new Battlefield.Position(0, 1);
        targetPosScreenspace = Battlefield.instance.GetSpaceScreenSpace(lastTargetPos);
        transform.position = targetPosScreenspace;
    }

    public float MoveSpeed = 0.5f; // Speed of target reticule movement (when selecting diff targets).
    private Battlefield.Position lastTargetPos;
    private Vector2 targetPosScreenspace;

    // Update is called once per frame
    void Update()
    {
        // Move scouter reticule
        SetTargetPos();
        if(transform.position.x != targetPosScreenspace.x || transform.position.y != targetPosScreenspace.y)
        {
            transform.position = Vector2.Lerp(transform.position, targetPosScreenspace, MoveSpeed);
        }
    }

    protected void SetTargetPos()
    {
        if (Battlefield.instance.Player == null)
            return;
        var targetFieldPos = Battlefield.instance.Player.TargetPos;
        if (lastTargetPos == targetFieldPos)
            return;
        lastTargetPos = new Battlefield.Position(targetFieldPos);
        targetPosScreenspace = Battlefield.instance.GetSpaceScreenSpace(targetFieldPos);
    }
}
