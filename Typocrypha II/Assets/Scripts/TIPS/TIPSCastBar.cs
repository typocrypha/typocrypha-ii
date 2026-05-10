//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using Typocrypha;
using UnityEngine.UI;

public class TIPSCastBar : CastBar
{
    public UnityEvent_string OnSearchCast;
    [SerializeField] AudioClip typeSfx;
    [SerializeField] Image frame;

    [SerializeField] Color colorDefault;
    [SerializeField] Color colorFocus;

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

    public override void Focus()
    {
        frame.color = colorFocus;
        base.Focus();
    }

    public override void Unfocus()
    {
        frame.color = colorDefault;
        base.Unfocus();
    }
}