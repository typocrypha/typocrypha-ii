using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace Gameflow
{
    [Node(false, "Gameflow/EndAndGoto", new System.Type[] { typeof(GameflowCanvas), typeof(DialogCanvas) })]
    public class EndAndGoto : GameflowEndNode
    {
        new public const string ID = "End And Goto Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "End And Goto"; } }
        public override Vector2 MinSize { get { return new Vector2(220, 60); } }
        public override bool AutoLayout { get { return true; } }

        #region Tooltip Strings
        protected const string tooltipGraph = "Next dialog graph to play";
        #endregion

        public DialogCanvas nextDialog; // Next dialog to transition to immediately.

        public override void NodeGUI()
        {
            GUILayout.Space(3);
            #region Graph
            GUILayout.Space(5);
            GUILayout.BeginVertical("Box");
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Dialog Graph", tooltipGraph), NodeEditorGUI.nodeLabel, GUILayout.Width(80f));
            nextDialog = RTEditorGUI.ObjectField(nextDialog, false, GUILayout.Width(MinSize.x - 100f));
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            #endregion
        }
    }
}
