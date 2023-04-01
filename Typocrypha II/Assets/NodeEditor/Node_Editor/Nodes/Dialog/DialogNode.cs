using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace Gameflow
{
    [Node(true, "Dialog/DialogBase", new System.Type[] { typeof(DialogCanvas)})]
    public abstract class DialogNode : BaseNodeIO
    {
        public override bool AllowRecursion { get { return true; } }

        public string characterName; // Name of character speaking (used by default to find speaking sfx).
        public string displayName; // Override display name (if name should be different from speaker's actual name).
        public string text; // Dialog line.
        public TIPSEntryData tipsData; // TIPS searchable metadata (if applicable).

        #region Tooltip Strings
        protected const string tooltipName = "The speaking character's name. Used to set speaking sfx and sprite highlighting if not overriden by text events";
        protected const string tooltipDisplayName = "The name displayed. Leave blank to use the speaker's name.";
        protected const string tooltipText = "The text to be displayed. Can substitute text macros using {macro-name,args}, and call text events using [event-name,args]";
        protected const string tooltipTIPS = "Metadata for TIPS-searchable term that comes up in dialog";
        #endregion

        protected override void OnCreate()
        {
            characterName = "Character Name";
            displayName = "";
            text = "Insert dialog text here";
        }

        // GUI for the speaking character's name.
        protected void NameGUI()
        {
            GUILayout.Space(5);
            GUILayout.BeginVertical("Box");
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Name", tooltipName), NodeEditorGUI.nodeLabel, GUILayout.Width(45f));
            characterName = GUILayout.TextField(characterName, GUILayout.Width(MinSize.x - 65));
            GUILayout.EndHorizontal();
            GUILayout.Space(3);
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Display", tooltipDisplayName), NodeEditorGUI.nodeLabel, GUILayout.Width(45f));
            displayName = GUILayout.TextField(displayName, GUILayout.Width(MinSize.x - 65));
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        // GUI for the dialog text field.
        protected void TextGUI()
        {
            GUILayout.Label(new GUIContent("Dialog Text", tooltipText), NodeEditorGUI.nodeLabelBoldCentered);
            GUILayout.BeginHorizontal();
            GUIStyle dialogTextStyle = new GUIStyle(GUI.skin.textArea);
            dialogTextStyle.wordWrap = true;
            text = GUILayout.TextArea(text, GUI.skin.textArea, GUILayout.MinHeight(RTEditorGUI.lineHeight * 5));
            GUILayout.EndHorizontal();
        }

        // GUI for TIPS-searchable entries
        protected void TIPSGUI()
        {
            GUILayout.Space(5);
            GUILayout.BeginVertical("Box");
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("TIPS Entry", tooltipTIPS), NodeEditorGUI.nodeLabel, GUILayout.Width(65f));
            tipsData = RTEditorGUI.ObjectField(tipsData, false, GUILayout.Width(MinSize.x - 85f));
            GUILayout.EndHorizontal();
            GUILayout.Space(3);
            GUILayout.EndVertical();
        }
    }
}
