using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;

namespace Gameflow
{
    public abstract class GameflowCanvas : NodeCanvas
    {
        public virtual GameflowStartNode GetStartNode()
        {
            foreach (var node in nodes)
            {
                if (node is GameflowStartNode startNode)
                {
                    return startNode;
                }
            }
            return null;
        }
    }
}
