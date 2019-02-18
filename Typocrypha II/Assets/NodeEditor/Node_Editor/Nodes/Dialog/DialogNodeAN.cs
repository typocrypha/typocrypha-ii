using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace Gameflow
{
    [Node(false, "Dialog/AN Dialog", new System.Type[] { typeof(GameflowCanvas), typeof(DialogCanvas) })]
    public class DialogNodeAN : DialogNode
    {
        #region Editor
        public const string ID = "AN Dialog Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "AN Dialog"; } }
        public override Vector2 MinSize { get { return new Vector2(250, 60); } }

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
            GUILayout.Space(3);
            GUILayout.EndVertical();

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
        }
        #endregion
    }

}