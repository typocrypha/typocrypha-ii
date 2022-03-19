using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

using System;
using Gameflow.GUIUtilities;

namespace Gameflow
{
    [Node(false, "Event/Character/Add Character", new System.Type[] { typeof(GameflowCanvas), typeof(DialogCanvas) })]
    public class AddCharacter : CharacterControlNode
    {
        public const string ID = "Add Character Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Add Character"; } }
        public override Vector2 MinSize { get { return new Vector2(250, 60); } }

        public Vector2 targetPos = new Vector2(0, 0);

        public DialogViewVNPlus.CharacterColumn column;

        #region Tooltip Strings
        protected const string tooltipPos = "Where to position character (center pivot)";
        #endregion

        protected override void OnCreate()
        {
            characterData = null;
            targetPos = Vector2.zero;
        }

        public override void NodeGUI()
        {
            base.NodeGUI();

            #region Position
            GUILayout.Label(new GUIContent("Position", tooltipPos), NodeEditorGUI.nodeLabelBoldCentered);
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUIStyle dialogTextStyle = new GUIStyle(GUI.skin.textArea);
            dialogTextStyle.wordWrap = true;
            targetPos = RTEditorGUI.Vector2Field(new Rect(4, 50, MinSize.x - 10, 20), "", targetPos);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            column = (DialogViewVNPlus.CharacterColumn)RTEditorGUI.EnumPopup(column);
            GUILayout.EndHorizontal();
            #endregion
        }
    }
}