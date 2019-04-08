using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;

namespace Gameflow
{
    [NodeCanvasType("Battle Canvas")]
    [CreateAssetMenu(fileName = "newBattle", menuName = "Battle")]
    public class BattleCanvas : NodeCanvas
    {
        public override string canvasName { get { return "Battle"; } }

        public GameflowStartNode getStartNode()
        {
            return nodes.Find((node) => { return node is BattleStartNode; }) as GameflowStartNode;
        }

    }

}
