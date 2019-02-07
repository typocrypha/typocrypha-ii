using UnityEngine;
using UnityEditor;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace Gameflow
{
    [Node(false, "Dialog/VN Dialog", new System.Type[] { typeof(GameflowCanvas), typeof(DialogCanvas) })]
    public class DialogNodeVN : DialogNode
    {
        #region Editor
        public const string ID = "VN Dialog Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "VN Dialog"; } }
        public override Vector2 MinSize { get { return new Vector2(250, 60); } }

        public string expression;

        #region Tooltip Strings
        private const string tooltip_name = "The speaking character's name. Used to set speaking sfx and sprite highlighting if not overriden by text events";
        private const string tooltip_expr = "The speaking character's expression. Only needs to be set if the expression should be changed";
        protected const string tooltip_text = "The text to be displayed. Can substitute text macros using {macro-name,args}, and call text events using [event-name,args]";
        #endregion

        protected override void OnCreate()
        {
            characterName = "Character Name";
            expression = "Expression";
            text = "Insert dialog text here";
        }

        public override void NodeGUI()
        {
            GUILayout.Space(5);
            GUILayout.BeginVertical("Box");
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Name", tooltip_name), GUILayout.Width(45f));
            characterName = GUILayout.TextField(characterName, GUILayout.Width(MinSize.x - 65));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Expr", tooltip_expr), NodeEditorGUI.nodeLabelLeft, GUILayout.Width(45f));
            expression = GUILayout.TextField(expression, GUILayout.Width(MinSize.x - 65));
            GUILayout.EndHorizontal();
            GUILayout.Space(3);
            GUILayout.EndVertical();

            GUILayout.Label(new GUIContent("Dialog Text", tooltip_text), NodeEditorGUI.nodeLabelBoldCentered);

            GUILayout.BeginHorizontal();
            GUIStyle dialogTextStyle = new GUIStyle(GUI.skin.textArea);
            dialogTextStyle.wordWrap = true;
            text = GUILayout.TextArea(text, GUI.skin.textArea,GUILayout.MinHeight(RTEditorGUI.lineHeight * 5));
            GUILayout.EndHorizontal();

            //Don't know why this code needs to be here exactly, but it makes everything nicer? maybe add to some static stuff?
            GUILayout.BeginHorizontal();
            RTEditorGUI.labelWidth = 90;
            GUILayout.EndHorizontal();
        }
        #endregion

    }
}
