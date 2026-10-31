using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class CustomLineRenderer : UILineRenderer
{
    protected override void OnEnable()
    {
        // Stop this component from yapping
        Debug.unityLogger.filterLogType -= LogType.Warning;
        base.OnEnable();
        Debug.unityLogger.filterLogType |= LogType.Warning;
    }
}
