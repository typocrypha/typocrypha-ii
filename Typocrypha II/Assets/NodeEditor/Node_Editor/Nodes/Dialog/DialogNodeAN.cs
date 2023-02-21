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

        public TMPro.TextAlignmentOptions alignmentOptions = TMPro.TextAlignmentOptions.Left;

        protected override void OnCreate()
        {
            base.OnCreate();
            characterName = "Name (optional)";
        }

        public override void NodeGUI()
        {
            NameGUI();
            TextGUI();
            alignmentOptions = (TMPro.TextAlignmentOptions)RTEditorGUI.EnumPopup(new GUIContent("Alignment"), alignmentOptions);

            //Don't know why this code needs to be here exactly, but it makes everything nicer? maybe add to some static stuff?
            GUILayout.BeginHorizontal();
            RTEditorGUI.labelWidth = 90;
            GUILayout.EndHorizontal();
        }
        #endregion
    }

}