using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace Gameflow
{
    [Node(false, "Dialog/Chat Dialog", new System.Type[] { typeof(GameflowCanvas), typeof(DialogCanvas) })]
    public class DialogNodeChat : DialogNode
    {
        #region Editor
        public const string ID = "Chat Dialog Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Chat Dialog"; } }
        public override Vector2 MinSize { get { return new Vector2(250, 60); } }

        public Sprite leftIcon;
        public Sprite rightIcon;

        #region Tooltip Strings
        private const string tooltipSprite = "Sprites on the left and right side of the chat box.";
        #endregion

        protected override void OnCreate()
        {
            base.OnCreate();
            characterName = "Name (optional)";
        }

        public override void NodeGUI()
        {
            NameGUI();
            TextGUI();

            //Don't know why this code needs to be here exactly, but it makes everything nicer? maybe add to some static stuff?
            GUILayout.BeginHorizontal();
            RTEditorGUI.labelWidth = 90;
            GUILayout.EndHorizontal();

            #region Icon Sprites
            GUILayout.BeginVertical("Box");
            GUILayout.Space(3);
            GUILayout.Label(new GUIContent("left       || Icons ||       right", tooltipSprite), NodeEditorGUI.nodeLabelBoldCentered);
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            leftIcon = RTEditorGUI.ObjectField(leftIcon, false, GUILayout.Width(65f), GUILayout.Height(65f));
            GUILayout.Space(MinSize.x - 170);
            rightIcon = RTEditorGUI.ObjectField(rightIcon, false, GUILayout.Width(65f), GUILayout.Height(65f));
            GUILayout.Space(10);
            GUILayout.EndHorizontal();
            GUILayout.Space(3);
            GUILayout.EndVertical();
            #endregion
        }
        #endregion
    }

}
