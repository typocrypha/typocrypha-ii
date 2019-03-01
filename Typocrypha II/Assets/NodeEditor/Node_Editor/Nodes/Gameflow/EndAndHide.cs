using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;

namespace Gameflow
{
    /// <summary>
    /// Hide the dialog canvas when dialog ends.
    /// </summary>
    [Node(false, "Gameflow/EndAndHide", new System.Type[] { typeof(GameflowCanvas), typeof(DialogCanvas) })]
    public class EndAndHide : GameflowEndNode
    {
        new public const string ID = "End And Hide Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "End And Hide"; } }
        public override Vector2 MinSize { get { return new Vector2(150, 60); } }
        public override bool AutoLayout { get { return true; } }
    }
}