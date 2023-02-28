using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace Gameflow
{
    [Node(false, "Event/Audio/Pause BGM", new System.Type[] { typeof(GameflowCanvas), typeof(DialogCanvas) })]
    public class PauseBgm : AudioControlNode
    {
        public const string ID = "Pause BGM Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Pause BGM"; } }
        public override Vector2 MinSize { get { return new Vector2(250, 60); } }

        public bool pause; // Whether to pause or unpause.

        #region Tooltip Strings
        const string tooltipPause = "Toggle on to pause, toggle off to unpause.";
        #endregion

        public override void NodeGUI()
        {
            #region Pause
            GUILayout.Space(5);
            GUILayout.BeginVertical("Box");
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Pause", tooltipPause), NodeEditorGUI.nodeLabel, GUILayout.Width(65f));
            pause = RTEditorGUI.Toggle(pause, new GUIContent());
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            #endregion
        }

    }
}