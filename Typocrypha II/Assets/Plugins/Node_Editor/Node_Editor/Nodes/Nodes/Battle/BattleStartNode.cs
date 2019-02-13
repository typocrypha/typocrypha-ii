﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;

namespace Gameflow
{
    [Node(false, "Battle/Start", new System.Type[] { typeof(GameflowCanvas) })]
    public class BattleStartNode : BattleNode
    {
        public enum TransitionType
        {
            Swirl,
        }

        public const string ID = "Battle Start Node";
        public override string GetID { get { return ID; } }

        public int numWaves;
        public TransitionType transitionType;
    }
}