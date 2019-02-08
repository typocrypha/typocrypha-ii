using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;

namespace Gameflow
{

    [Node(true, "Gameflow/Standard Output Base", new System.Type[] { typeof(GameflowCanvas) })]
    public abstract class BaseNodeOUT : BaseNode
    {
        //Next Node to go to (OUTPUT)
        [ConnectionKnob("To Next", Direction.Out, "Gameflow", ConnectionCount.Single, NodeSide.Right, 30)]
        public ConnectionKnob toNextOUT;
        public BaseNode Next { get => toNextOUT.connections[0].body as BaseNode; }
    }
}
