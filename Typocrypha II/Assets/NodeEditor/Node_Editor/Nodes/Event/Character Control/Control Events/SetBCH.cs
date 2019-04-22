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
    [Node(false, "Event/Character/Set Body-Clothes-Hair", new System.Type[] { typeof(GameflowCanvas), typeof(DialogCanvas) })]
    public class SetBCH : CharacterControlNode
    {
        public const string ID = "Set Body-Clothes-Hair Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Set Body-Clothes-Hair"; } }
        public override Vector2 MinSize { get { return new Vector2(250, 60); } }

        public string body;
        public string clothes;
        public string hair;

        #region Tooltip Strings
        protected const string tooltip = "Name of feature (leave blank for no change)";
        #endregion

        protected override void OnCreate()
        {
            characterData = null;
            body = "Body";
            clothes = "Clothes";
            hair = "Hair";
        }

        public override void NodeGUI()
        {
            base.NodeGUI();

            #region Features
            GUIStyle dialogTextStyle = new GUIStyle(GUI.skin.textArea);
            dialogTextStyle.wordWrap = true;

            GUILayout.Label(new GUIContent("Body", tooltip), NodeEditorGUI.nodeLabelBoldCentered);
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            body = GUILayout.TextField(body, GUILayout.Width(MinSize.x - 10));
            GUILayout.Space(5);
            GUILayout.EndHorizontal();

            GUILayout.Label(new GUIContent("Clothes", tooltip), NodeEditorGUI.nodeLabelBoldCentered);
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            clothes = GUILayout.TextField(clothes, GUILayout.Width(MinSize.x - 10));
            GUILayout.Space(5);
            GUILayout.EndHorizontal();

            GUILayout.Label(new GUIContent("Hair", tooltip), NodeEditorGUI.nodeLabelBoldCentered);
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            hair = GUILayout.TextField(hair, GUILayout.Width(MinSize.x - 10));
            GUILayout.Space(5);
            GUILayout.EndHorizontal();
            #endregion
        }
    }
}

