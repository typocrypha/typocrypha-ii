using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gameflow
{
    [Node(false, "Misc/Note", new System.Type[] { typeof(GameflowCanvas), typeof(DialogCanvas), typeof(BattleCanvas) })]
    public class NoteNode : BaseNode
    {
        public const string ID = "Note Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Note"; } }
        [SerializeField] private Vector2 extraSize;
        public override Vector2 MinSize { get => new Vector2(250, 100) + extraSize; }

        public string text;

        public override void NodeGUI()
        {
#if UNITY_EDITOR
            extraSize = EditorGUILayout.Vector2Field(new GUIContent("Extra Space"), extraSize);
            text = GUILayout.TextArea(text, GUILayout.Height(extraSize.y + 70));
            GUILayout.BeginHorizontal();
            RTEditorGUI.labelWidth = 90;
            GUILayout.EndHorizontal();
#endif
        }
        protected override void OnCreate()
        {
            extraSize = new Vector2(0, 0);
        }
    }
}
