using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace Gameflow
{
    [Node(false, "Dialog/Chat Dialog", new System.Type[] { typeof(GameflowCanvas), typeof(DialogCanvas) })]
    public class DialogNodeChat : DialogNode
    {
        #region Editor
        public const string ID = "Chat Dialog Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Chat Dialog"; } }
        public override Vector2 MinSize { get { return new Vector2(250, 60); } }

        public Sprite leftIcon;
        public Sprite rightIcon;

        #region Tooltip Strings
        private const string tooltip_name = "The speaking character's name (optional). Used to set speaking sfx if not overriden by text events";
        protected const string tooltip_text = "The text to be displayed. Can substitute text macros using {macro-name,args}, and call text events using [event-name,args]";
        #endregion

        protected override void OnCreate()
        {
            characterName = "Name (optional)";
            text = "Insert dialog text here";
        }

        public override void NodeGUI()
        {
            GUILayout.Space(5);
            GUILayout.BeginVertical("Box");
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Name", tooltip_name), NodeEditorGUI.nodeLabel, GUILayout.Width(45f));
            characterName = GUILayout.TextField(characterName, GUILayout.Width(MinSize.x - 65));
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            #region Text Field
            GUILayout.Label(new GUIContent("Dialog Text", tooltip_text), NodeEditorGUI.nodeLabelBoldCentered);
            GUILayout.BeginHorizontal();
            GUIStyle dialogTextStyle = new GUIStyle(GUI.skin.textArea);
            dialogTextStyle.wordWrap = true;
            text = GUILayout.TextArea(text, dialogTextStyle, GUILayout.MinHeight(RTEditorGUI.lineHeight * 5));
            GUILayout.EndHorizontal();
            //Don't know why this code needs to be here exactly, but it makes everything nicer? maybe add to some static stuff?
            GUILayout.BeginHorizontal();
            RTEditorGUI.labelWidth = 90;
            GUILayout.EndHorizontal();
            #endregion

            #region Icon Sprites
            GUILayout.BeginVertical("Box");
            GUILayout.Space(3);
            GUILayout.Label(new GUIContent("left       || Icons ||       right", tooltip_name), NodeEditorGUI.nodeLabelBoldCentered);
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            leftIcon = RTEditorGUI.ObjectField(leftIcon, false, GUILayout.Width(65f), GUILayout.Height(65f));
            GUILayout.Space(MinSize.x - 170);
            rightIcon = RTEditorGUI.ObjectField(rightIcon, false, GUILayout.Width(65f), GUILayout.Height(65f));
            GUILayout.Space(10);
            GUILayout.EndHorizontal();
            GUILayout.Space(3);
            GUILayout.EndVertical();
            #endregion
        }
        #endregion
    }

}
