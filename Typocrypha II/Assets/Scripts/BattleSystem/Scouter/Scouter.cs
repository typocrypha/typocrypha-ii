using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Target reticule and scouter during battle.
/// </summary>
public class Scouter : MonoBehaviour, IPausable
{
    #region IPausable
    PauseHandle ph;
    public PauseHandle PH => ph;
    // Stop target reticule movement and animation.
    public void OnPause(bool pause)
    {
        enabled = !pause;
        GetComponent<Animator>().speed = pause ? 0f : 1f;
    }
    #endregion

    void Awake()
    {
        ph = new PauseHandle(OnPause);
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
        transform.position = Vector2.Lerp(transform.position, TargetPos, MoveSpeed);
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            var obj = Battlefield.instance.GetObject(Battlefield.instance.Player.TargetPos);
            Debug.Log(obj.GetScouterInfo().DescriptionText);
        }
    }
}
