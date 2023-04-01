using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;

namespace Gameflow
{
    [NodeCanvasType("Battle Canvas")]
    [CreateAssetMenu(fileName = "newBattle", menuName = "Battle")]
    public class BattleCanvas : GameflowCanvas
    {
        public override string canvasName => "Battle";

        public override GameflowStartNode GetStartNode()
        {
            foreach (var node in nodes)
            {
                if (node is BattleStartNode startNode)
                {
                    return startNode;
                }
            }
            return null;
        }
    }

}
