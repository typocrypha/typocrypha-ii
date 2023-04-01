using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;

namespace Gameflow
{

    [NodeCanvasType("Dialog Canvas")]
    [CreateAssetMenu(fileName ="newScene", menuName ="Dialog Scene")]
    public class DialogCanvas : GameflowCanvas
    {
        public override string canvasName => "Dialog";
    }
}
