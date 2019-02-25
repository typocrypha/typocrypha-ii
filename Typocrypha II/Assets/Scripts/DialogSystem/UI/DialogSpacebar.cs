using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages behavior of spacebar icon during dialog.
/// </summary>
[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public class DialogSpacebar : MonoBehaviour
{
    /// <summary>
    /// Spacebar display states.
    /// </summary>
    public enum State
    {
        skip, // Pressing space will skip current text scroll and display whole line.
        next, // Pressing space will go to next line of dialog.
        blocked // Cannot press space.
    }
    // Map enum state to animator trigger label.
    Dictionary<State, string> stateMap = new Dictionary<State, string>
    {
        {State.skip, "Skip" },
        {State.next, "Next" },
        {State.blocked, "Blocked" },
    };

    static DialogSpacebar curr = null;
    public static DialogSpacebar Curr // Currently active spacebar.
    {
        get => curr;
    }
    State currState;
    public State CurrState // Spacebar press state.
    {
        get => currState;
        set
        {
            currState = value;
            animator.SetTrigger(stateMap[value]);
        }
    }

    SpriteRenderer sr; // Sprite renderer for spacebar.
    Animator animator; // Animator for spacebar.

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        curr = this;
    }

    // Check dialog state and update spacebar state.
    void Update()
    {
        State newState = CurrState;
        if (DialogManager.instance.PH.Pause) newState = State.blocked;
        else
        {
            if (DialogManager.instance.dialogBox.IsDone) newState = State.next;
            else newState = State.skip;
        }
        if (newState != CurrState) CurrState = newState;
    }
}
