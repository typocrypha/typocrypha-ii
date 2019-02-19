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
    [Node(false, "Event/Character/Set Expression", new System.Type[] { typeof(GameflowCanvas), typeof(DialogCanvas) })]
    public class SetExpression : CharacterControlNode
    {
        public const string ID = "Set Expression Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Set Expression"; } }
        public override Vector2 MinSize { get { return new Vector2(250, 60); } }

        public string expr; // New expression

        #region Tooltip Strings
        protected const string tooltipExpr = "Name of new expression";
        #endregion

        protected override void OnCreate()
        {
            characterData = null;
            expr = "Expression";
        }

        public override void NodeGUI()
        {
            base.NodeGUI();

            #region Expressions
            GUILayout.Label(new GUIContent("Expression", tooltipExpr), NodeEditorGUI.nodeLabelBoldCentered);
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            GUIStyle dialogTextStyle = new GUIStyle(GUI.skin.textArea);
            dialogTextStyle.wordWrap = true;
            expr = GUILayout.TextField(expr, GUILayout.Width(MinSize.x - 10));
            GUILayout.Space(5);
            GUILayout.EndHorizontal();
            #endregion
        }
    }
}
