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

        public override Vector2 MinSize => new Vector2(250, 100);

        new public const string ID = "Battle Start Node";
        public override string GetID { get { return ID; } }
        public override string Title => "Battle Start";

        public GameObject player;
        public string totalWaves;
        public CharacterData initialAllyData;
        public string initialAllyPose;
        public string initialAllyExpr;
        public TransitionType transitionType;

        public override void NodeGUI()
        {
            GUILayout.Space(3);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Player", GUILayout.Width(100));
            player = RTEditorGUI.ObjectField(player, false, GUILayout.Width(125));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Total Waves", GUILayout.Width(100));
            totalWaves = RTEditorGUI.TextField(totalWaves, GUILayout.Width(125));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Ally Character", GUILayout.Width(100));
            initialAllyData = RTEditorGUI.ObjectField(initialAllyData, false, GUILayout.Width(125));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Initial Ally Expr"), GUILayout.Width(100));
            initialAllyExpr = RTEditorGUI.TextField(initialAllyExpr);
            GUILayout.Space(5);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Initial Ally Pose"), GUILayout.Width(100));
            initialAllyPose = RTEditorGUI.TextField(initialAllyPose);
            GUILayout.Space(5);
            GUILayout.EndHorizontal();
        }
    }
}
