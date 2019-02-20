
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace Gameflow
{
    [Node(false, "Dialog/Dialog Input", new System.Type[] { typeof(GameflowCanvas), typeof(DialogCanvas) })]
    public class DialogNodeInput : DialogNodeVN
    {
        #region Editor
        new public const string ID = "Dialog Input Node";
        public override string GetID { get { return ID; } }
        public override string Title { get { return "Dialog Input"; } }

        public string variableName;
        public bool showChoicePrompt;
        public string[] choicePromptText;

        protected const string tooltip_saveTo = "Name of the variable to save the input to. Can be set as \"none\" to not save input";
        protected const string tooltip_showPrompt = "Show input prompts (AB w/ optional C). The choice will still branch arbitrarily";

        protected override void OnCreate()
        {
            base.OnCreate();
            characterName = "PROMPT";
            variableName = "variable-name";
            showChoicePrompt = false;
            choicePromptText = new string[3];
        }

        public override void NodeGUI()
        {
            base.NodeGUI();
            GUILayout.BeginVertical("box");
            GUILayout.Label(new GUIContent("Save Input To Variable", tooltip_saveTo), NodeEditorGUI.nodeLabelBoldCentered);
            variableName = GUILayout.TextField(variableName);
            GUILayout.Space(5);
            GUILayout.EndVertical();
            GUILayout.BeginVertical("box");
            showChoicePrompt = RTEditorGUI.Toggle(showChoicePrompt, "Show Choice Prompts?", NodeEditorGUI.nodeLabelBoldCentered);
            //Extra block that can be toggled on and off.
            if (showChoicePrompt)
            {
                RTEditorGUI.indent++;
                choicePromptText[0] = GUILayout.TextField(choicePromptText[0]);
                choicePromptText[1] = GUILayout.TextField(choicePromptText[1]);
                choicePromptText[2] = GUILayout.TextField(choicePromptText[2]);
                RTEditorGUI.indent--;
            }
            GUILayout.EndVertical();
        }
        #endregion

    }
}
