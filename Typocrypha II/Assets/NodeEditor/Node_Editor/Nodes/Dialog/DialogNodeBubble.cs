using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gameflow
{
    [Node(false, "Dialog/Bubble Dialog", new System.Type[] { typeof(DialogCanvas) })]
    public class DialogNodeBubble : DialogNode
    {
        #region Editor
        public const string ID = "Bubble Dialog Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Bubble Dialog"; } }
        public override Vector2 MinSize { get { return new Vector2(250, 60); } }

        public Vector2Int gridPosition;
        public Vector2 absolutePosition = DialogViewBubble.nullAbsolutePos;


        protected override void OnCreate()
        {
            base.OnCreate();
            characterName = "Name (optional)";
        }

        public override void NodeGUI()
        {
            NameGUI();
            TextGUI();
            
            #if UNITY_EDITOR
            gridPosition = EditorGUILayout.Vector2IntField("Grid Pos", gridPosition);
            absolutePosition = EditorGUILayout.Vector2Field("Absolute Pos", absolutePosition);
            #endif
            //Don't know why this code needs to be here exactly, but it makes everything nicer? maybe add to some static stuff?
            GUILayout.BeginHorizontal();
            RTEditorGUI.labelWidth = 90;
            GUILayout.EndHorizontal();
        }
        #endregion
    }

}