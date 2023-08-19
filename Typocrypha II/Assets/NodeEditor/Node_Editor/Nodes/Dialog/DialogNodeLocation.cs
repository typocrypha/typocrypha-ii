using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace Gameflow
{
    [Node(false, "Dialog/Location Dialog", new System.Type[] { typeof(DialogCanvas) })]
    public class DialogNodeLocation : DialogNode
    {
        #region Editor
        public const string ID = "Location Dialog Node";
        public override string GetID => ID;

        public override string Title => "Location Dialog";
        public override Vector2 MinSize => new Vector2(250, 60);

        protected override void OnCreate()
        {
            base.OnCreate();
            characterName = "Name";
        }

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