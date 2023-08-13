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
        public static readonly Dictionary<string, float> variables = new Dictionary<string, float>()
        {
            { "loadingFade", LoadingScreenDefault.fadeTime / 2 }
        };
        public override string Title => "Wait";

        public const string ID = "wait";
        public override string GetID => ID;

        public float WaitTime
        {
            get
            {
                if (!string.IsNullOrEmpty(variableName) && variables.TryGetValue(variableName, out var variable))
                    return variable;
                return duration;
            }
        }

        public float duration = 1f;
        public string variableName = "";

        public override void NodeGUI()
        {
            GUILayout.BeginVertical("box");
            duration = RTEditorGUI.FloatField("Duration", duration);
            GUILayout.Label(new GUIContent($"Variable Name:"), NodeEditorGUI.nodeLabelBoldCentered);
            variableName = GUILayout.TextField(variableName);
            GUILayout.EndVertical();
        }
    }
}
