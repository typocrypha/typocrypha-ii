//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using Typocrypha;
using UnityEngine.Events;

public class TIPSCastBar : CastBar
{
    public UnityEvent_string OnSearchCast;
    [SerializeField] AudioClip typeSfx;

    protected override int VisualKeywordDelimiterIndex => delimiterIndexSpace;

    public void ProcessInput(string input)
    {
        var validInput = CheckInput(input);
        if (validInput.HasValue && validInput.Value)
        {
            AudioManager.instance.PlaySFX(typeSfx);
        }
    }

    public override void Submit()
    {
        OnSearchCast.Invoke(Text);
        Clear();
    }
}