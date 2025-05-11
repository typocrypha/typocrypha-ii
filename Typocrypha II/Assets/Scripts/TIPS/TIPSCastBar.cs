//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using Typocrypha;
using UnityEngine.Events;

public class TIPSCastBar : CastBar
{
    public UnityEvent_string OnSearchCast;
    public UnityEvent OnValidKeyPressed;
    [SerializeField] AudioClip typeSfx;

    private void Update()
    {
        ProcessInput(Input.inputString);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Submit();
            Clear();
        }
    }

    public void ProcessInput(string input)
    {
        var validInput = CheckInput(input);
        if (validInput.HasValue && validInput.Value)
        {
            AudioManager.instance.PlaySFX(typeSfx);
            OnValidKeyPressed.Invoke();
        }
    }

    public override void Submit()
    {
        OnSearchCast.Invoke(Text);
    }
}