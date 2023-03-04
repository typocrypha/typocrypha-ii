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
    }

    public float MoveSpeed = 0.5f; // Speed of target reticule movement (when selecting diff targets).
    Vector2 TargetPos // Position of player target.
    {
        get => Battlefield.instance.Player != null
            ? Battlefield.instance.GetSpace(Battlefield.instance.Player.TargetPos)
            : Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        // Move scouter reticule
        transform.position = Vector2.Lerp(transform.position, TargetPos, MoveSpeed);
    }
}
