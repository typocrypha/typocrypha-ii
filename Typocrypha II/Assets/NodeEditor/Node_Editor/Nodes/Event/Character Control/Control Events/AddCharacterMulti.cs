using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace Gameflow
{
    [Node(false, "Event/Character/Add Character (Multi)", new System.Type[] { typeof(DialogCanvas) })]
    public class AddCharacterMulti : MultiCharacterControlNode
    {
        public const string ID = "Add Character Node Multi";
        public override string GetID => ID;

        public override string Title => "Add Characters";
        public override Vector2 MinSize => new Vector2(250, 60);

        public DialogView.CharacterColumn column;
        public string initialPose1;
        public string initialPose2;
        public string initialPose3;
        public string initialExpr1;
        public string initialExpr2;
        public string initialExpr3;

        #region Tooltip Strings
        protected const string tooltipPos = "Where to position character (center pivot)";
        #endregion

        protected override void OnCreate()
        {
            characterData1 = null;
        }

        public override void NodeGUI()
        {
            base.NodeGUI();
            GUILayout.BeginHorizontal();
            column = (DialogView.CharacterColumn)RTEditorGUI.EnumPopup(column);
            GUILayout.EndHorizontal();
            DataUI(ref initialExpr1, ref initialPose1, 1);
            DataUI(ref initialExpr2, ref initialPose2, 2);
            DataUI(ref initialExpr3, ref initialPose3, 3);
        }

        private void DataUI(ref string initialExpr, ref string initialPose, int number)
        {
            GUILayout.BeginHorizontal();
            GUIStyle dialogTextStyle = new GUIStyle(GUI.skin.textArea);
            dialogTextStyle.wordWrap = true;
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent($"Initial Expr {number}"), GUILayout.Width(100));
            initialExpr = RTEditorGUI.TextField(initialExpr);
            GUILayout.Space(5);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent($"Initial Pose {number}"), GUILayout.Width(100));
            initialPose = RTEditorGUI.TextField(initialPose);
            GUILayout.Space(5);
            GUILayout.EndHorizontal();
        }
    }
}