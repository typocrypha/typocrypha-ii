using Gameflow;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameflow
{
    [Node(false, "Event/Wait", new System.Type[] { typeof(DialogCanvas), typeof(BattleCanvas) })]
    public class PauseNode : BaseNodeIO
    {
        public override string Title => "Wait";
        public float duration = 1f;

        public const string ID = "wait";
        public override string GetID => ID;

        public override void NodeGUI()
        {
            GUILayout.BeginVertical("box");
            duration = RTEditorGUI.FloatField("Duration", duration);
            GUILayout.EndVertical();
        }
    }
}
