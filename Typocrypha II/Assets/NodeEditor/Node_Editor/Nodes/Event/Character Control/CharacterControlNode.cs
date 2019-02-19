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
    [Node(true, "Event/Character/Character Control Base", new System.Type[] { typeof(GameflowCanvas), typeof(DialogCanvas) })]
    public abstract class CharacterControlNode : BaseNodeIO
    {
        public override bool AllowRecursion { get { return true; } }

        public CharacterData characterData;

        #region Tooltip Strings
        protected const string tooltipData = "The selected character's CharacterData";
        #endregion

        public override void NodeGUI()
        {
            #region Data
            GUILayout.Space(5);
            GUILayout.BeginVertical("Box");
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            GUILayout.Label(new GUIContent("Character", tooltipData), NodeEditorGUI.nodeLabel, GUILayout.Width(65f));
            characterData = RTEditorGUI.ObjectField(characterData, false, GUILayout.Width(MinSize.x - 100f));
            GUILayout.Space(5);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            #endregion
        }
    }
}
