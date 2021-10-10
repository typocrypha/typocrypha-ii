using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace Gameflow
{
    [Node(false, "Dialog/VN Plus Dialog", new System.Type[] { typeof(GameflowCanvas), typeof(DialogCanvas) })]
    public class DialogNodeVNPlus : DialogNode
    {
        #region Editor
        public const string ID = "VN Plus Dialog Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "VN Plus Dialog"; } }
        public override Vector2 MinSize { get { return new Vector2(250, 60); } }

        public override void NodeGUI()
        {
            NameGUI();
            TextGUI();

            //Don't know why this code needs to be here exactly, but it makes everything nicer? maybe add to some static stuff?
            GUILayout.BeginHorizontal();
            RTEditorGUI.labelWidth = 90;
            GUILayout.EndHorizontal();
        }
        #endregion
    }
}
