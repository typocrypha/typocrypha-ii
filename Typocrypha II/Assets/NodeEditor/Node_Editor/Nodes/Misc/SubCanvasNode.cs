using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace Gameflow
{
    [Node(false, "Misc/SubCanvas", new System.Type[] { typeof(DialogCanvas), typeof(BattleCanvas) })]
    public class SubCanvasNode : BaseNodeIO
    {
        public const string ID = "SubCanvas Node";
        public override string GetID { get { return ID; } }
        public override string Title { get { return "SubCanvas"; } }
        public override Vector2 MinSize { get => new Vector2(250, 50); }

        public NodeCanvas subCanvas;

        public override void NodeGUI()
        {
            GUILayout.BeginHorizontal();
            subCanvas = RTEditorGUI.ObjectField<NodeCanvas>(subCanvas, false);
            GUILayout.EndHorizontal();
        }

        protected override void OnCreate()
        {
            base.OnCreate();
        }
    }
}

