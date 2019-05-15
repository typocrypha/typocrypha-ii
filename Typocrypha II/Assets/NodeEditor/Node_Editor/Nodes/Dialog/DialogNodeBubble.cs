using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace Gameflow
{
    [Node(false, "Dialog/Bubble Dialog", new System.Type[] { typeof(GameflowCanvas), typeof(DialogCanvas) })]
    public class DialogNodeBubble : DialogNode
    {
        #region Editor
        public const string ID = "Bubble Dialog Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Bubble Dialog"; } }
        public override Vector2 MinSize { get { return new Vector2(250, 60); } }

        public Rect rectVal;
        public bool multi = false; // Gathers current bubble with next bubble

        protected override void OnCreate()
        {
            base.OnCreate();
            characterName = "Name (optional)";
        }

        public override void NodeGUI()
        {
            NameGUI();
            TextGUI();
            
            GUILayout.Space(50);
            rectVal = RTEditorGUI.RectField(new Rect(20, 140, MinSize.x - 65, 20), "Rect", rectVal);
            
            multi = RTEditorGUI.Toggle(multi, "Multiple Mode");

            //Don't know why this code needs to be here exactly, but it makes everything nicer? maybe add to some static stuff?
            GUILayout.BeginHorizontal();
            RTEditorGUI.labelWidth = 90;
            GUILayout.EndHorizontal();
        }
        #endregion
    }

}