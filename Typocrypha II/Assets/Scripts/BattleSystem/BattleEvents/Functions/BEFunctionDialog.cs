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
        DialogManager.instance.StartDialog(graph, true);
    }
}
