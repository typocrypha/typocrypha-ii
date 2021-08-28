using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogViewVNPlus : DialogView
{
    public override void CleanUp()
    {

    }

    public override DialogBox PlayDialog(DialogItem data)
    {
        return null;
    }

    public override void SetEnabled(bool e)
    {
        gameObject.SetActive(e);
    }
}
