using UnityEngine;
using NodeEditorFramework;

namespace Gameflow
{
    [Node(false, "Event/Character/Set Pose", new System.Type[] { typeof(GameflowCanvas), typeof(DialogCanvas) })]
    public class SetPose : CharacterControlNode
    {
        public const string ID = "Set Pose Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Set Pose"; } }
        public override Vector2 MinSize { get { return new Vector2(250, 60); } }

        public string pose; // New pose.

        #region Tooltip Strings
        protected const string tooltipPose = "Name of new pose";
        #endregion

        protected override void OnCreate()
        {
            characterData = null;
            pose = "Pose";
        }

        public override void NodeGUI()
        {
            base.NodeGUI();

            #region Pose
            GUILayout.Label(new GUIContent("Pose", tooltipPose), NodeEditorGUI.nodeLabelBoldCentered);
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            GUIStyle dialogTextStyle = new GUIStyle(GUI.skin.textArea);
            dialogTextStyle.wordWrap = true;
            pose = GUILayout.TextField(pose, GUILayout.Width(MinSize.x - 10));
            GUILayout.Space(5);
            GUILayout.EndHorizontal();
            #endregion
        }
    }
}

