using UnityEngine;
using NodeEditorFramework;

namespace Gameflow
{
    /// <summary>
    /// Transition to a new scene when dialog ends.
    /// </summary>
    [Node(false, "Gameflow/EndAndTransition", new System.Type[] { typeof(GameflowCanvas), typeof(DialogCanvas), typeof(BattleCanvas)})]
    public class EndAndTransition : GameflowEndNode
    {
        new public const string ID = "End And Transition Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "End And Transition"; } }
        public override Vector2 MinSize { get { return new Vector2(220, 70); } }
        public override bool AutoLayout { get { return true; } }
    }
}
