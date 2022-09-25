using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameflow;

/// <summary>
/// Start a dialog scene.
/// </summary>
public class BEFunctionDialog : BattleEventFunction
{
    public DialogCanvas graph; // Dialog graph to use.

    public override void Run()
    {
        BattleManager.instance.PH.Pause = true;
        DialogManager.instance.StartDialog(graph);
        StartCoroutine(WaitForDialogEnd());
    }

    // Wait for dialog to end to unpause battle
    IEnumerator WaitForDialogEnd()
    {
        yield return new WaitUntil(() => DialogManager.instance.PH.Pause);
        BattleManager.instance.PH.Pause = false;
    }
}
