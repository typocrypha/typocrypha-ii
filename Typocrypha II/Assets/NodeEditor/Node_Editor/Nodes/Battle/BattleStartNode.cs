using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace Gameflow
{
    [Node(false, "Battle/Start", new System.Type[] { typeof(GameflowCanvas), typeof(BattleCanvas) })]
    public class BattleStartNode : GameflowStartNode
    {
        public enum TransitionType
        {
            Swirl,
        }

        new public const string ID = "Battle Start Node";
        public override string GetID { get { return ID; } }
        public override string Title => "Battle Start";

        public int numWaves;
        public TransitionType transitionType;

        public override void NodeGUI()
        {
            GUILayout.BeginHorizontal();
            RTEditorGUI.labelWidth = 90;
            GUILayout.EndHorizontal();
        }
    }
}
