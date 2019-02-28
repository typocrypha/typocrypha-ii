using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Prints a message to console.
/// </summary>
public class BEFunctionPrint : BattleEventFunction
{
    public string message = "Hello, world!";

    public override void Run()
    {
        Debug.Log(message);
    }
}
