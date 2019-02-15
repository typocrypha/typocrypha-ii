using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;

namespace Gameflow
{

    [NodeCanvasType("Dialog Canvas")]
    [CreateAssetMenu(fileName ="newScene", menuName ="Dialog Scene")]
    public class DialogCanvas : NodeCanvas
    {
        public override string canvasName { get { return "Dialog"; } }

        public GameflowStartNode getStartNode()
        {
            return nodes.Find((node) => { return node is GameflowStartNode; }) as GameflowStartNode;
        }

    }
}
