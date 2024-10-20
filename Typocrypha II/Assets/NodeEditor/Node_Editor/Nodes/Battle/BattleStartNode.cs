﻿using System.Collections;
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

        public override Vector2 MinSize => new Vector2(200, 100);

        new public const string ID = "Battle Start Node";
        public override string GetID { get { return ID; } }
        public override string Title => "Battle Start";

        public GameObject player;
        public int numWaves;
        public TransitionType transitionType;

        public override void NodeGUI()
        {
            GUILayout.Space(3);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Player", GUILayout.Width(40));
            player = RTEditorGUI.ObjectField(player, false, GUILayout.Width(150));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.EndHorizontal();
        }
    }
}
