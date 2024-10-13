//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Typocrypha;

public class TIPSCastBar : CastBar
{

    private void Update()
    {
        var validInput = CheckInput(Input.inputString);
        if (validInput.HasValue)
        {
            //if (validInput.Value) HasFocus = true;

        }

        //if (HasFocus && Input.GetKeyDown(KeyCode.Space)) Submit();
        //if (HasFocus && Input.GetKeyDown(KeyCode.DownArrow))
        //{

        //}
    }

    public override void Submit()
    {
        throw new System.NotImplementedException();
    }
}