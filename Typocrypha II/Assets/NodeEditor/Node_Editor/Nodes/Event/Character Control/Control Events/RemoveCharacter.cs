using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

using System;
using Gameflow.GUIUtilities;

namespace Gameflow
{
    [Node(false, "Event/Character/Remove Character", new System.Type[] { typeof(GameflowCanvas), typeof(DialogCanvas) })]
    public class RemoveCharacter : CharacterControlNode
    {
        public const string ID = "Remove Character Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Remove Character"; } }
        public override Vector2 MinSize { get { return new Vector2(250, 60); } }

        protected override void OnCreate()
        {
            characterData = null;
        }

        public override void NodeGUI()
        {
            base.NodeGUI();
        }
    }
}
