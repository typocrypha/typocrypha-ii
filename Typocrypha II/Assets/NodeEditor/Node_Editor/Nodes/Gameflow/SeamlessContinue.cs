using UnityEngine;
using NodeEditorFramework;

namespace Gameflow
{
    /// <summary>
    /// Continue to a new battle scene without interruption
    /// </summary>
    [Node(false, "Gameflow/SeamlessContinue", new System.Type[] { typeof(DialogCanvas), typeof(BattleCanvas)})]
    public class SeamlessContinue : GameflowEndNode
    {
        new public const string ID = "Seamless Continue Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Seamless Continue"; } }
        public override Vector2 MinSize { get { return new Vector2(220, 70); } }
        public override bool AutoLayout { get { return true; } }
    }
}
