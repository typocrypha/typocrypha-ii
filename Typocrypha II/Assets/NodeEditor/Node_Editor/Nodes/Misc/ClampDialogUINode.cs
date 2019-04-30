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
    /// <summary>
    /// Clamps Visual novel UI. 
    /// Moves screen frame up on/off screen and dialog box down on/off screen.
    /// </summary>
    [Node(false, "Event/Misc/Clamp Dialog UI", new System.Type[] { typeof(GameflowCanvas), typeof(DialogCanvas) })]
    public class ClampDialogUINode : BaseNodeIO
    {
        public const string ID = "Clamp Dialog UI Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Clamp Dialog UI"; } }
        public override Vector2 MinSize { get { return new Vector2(250, 60); } }

        public bool inOut; // Whether to clamp in or out

        #region Tooltip Strings
        const string tooltipToggle = "Toggle on to hide UI. Toggle off to show.";
        #endregion

        public override void NodeGUI()
        {
            #region Pause
            GUILayout.Space(5);
            GUILayout.BeginVertical("Box");
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Hide UI", tooltipToggle), NodeEditorGUI.nodeLabel, GUILayout.Width(65f));
            inOut = RTEditorGUI.Toggle(inOut, new GUIContent());
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            #endregion
        }

    }
}
