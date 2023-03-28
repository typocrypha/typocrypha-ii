using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace Gameflow
{
    [Node(false, "Event/Character/Move Character", new System.Type[] { typeof(GameflowCanvas), typeof(DialogCanvas) })]
    public class MoveCharacter : CharacterControlNode
    {
        public const string ID = "Move Character Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Move Character"; } }
        public override Vector2 MinSize { get { return new Vector2(250, 60); } }

        public DialogViewVNPlus.CharacterColumn targetColumn;
        public bool top;

        protected override void OnCreate()
        {
            base.OnCreate();
            top = false;
            targetColumn = DialogViewVNPlus.CharacterColumn.Left;
        }

        public override void NodeGUI()
        {
            base.NodeGUI();

            GUILayout.Label("Target Column");
            targetColumn = (DialogViewVNPlus.CharacterColumn)RTEditorGUI.EnumPopup(targetColumn);

            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            top = RTEditorGUI.Toggle(top, "Move To Top");
            GUILayout.EndHorizontal();
        }

    }

}
