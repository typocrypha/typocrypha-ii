using UnityEngine;
using NodeEditorFramework;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gameflow
{
    [Node(false, "Event/Audio/Pause BGM", new System.Type[] { typeof(DialogCanvas) })]
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
#if UNITY_EDITOR
        public override void NodeGUI()
        {
            #region Pause
            GUILayout.BeginVertical("Box");
            pause = EditorGUILayout.Toggle(new GUIContent("Pause", tooltipPause), pause);
            GUILayout.EndVertical();
            #endregion
        }
#endif
    }
}