using UnityEngine;
using NodeEditorFramework;

namespace Gameflow
{

    [Node(false, "Event/Set Location Text", new System.Type[] { typeof(GameflowCanvas), typeof(DialogCanvas) })]
    public class SetLocationTextNode : BaseNodeIO
    {
        public override string Title { get { return "Set Location Text"; } }
        public override Vector2 MinSize { get { return new Vector2(150, 50); } }
        public override bool AutoLayout { get { return true; } }

        public const string Id = "Set Location Text Node";
        public override string GetID { get { return Id; } }

        public string text;

        public override void NodeGUI()
        {
            GUILayout.Space(5);
            GUILayout.BeginVertical("box");
            GUILayout.Label(new GUIContent("Location Text"), NodeEditorGUI.nodeLabelBoldCentered);
            text = GUILayout.TextField(text);
            GUILayout.EndVertical();
        }

        protected override void OnCreate()
        {
            text = string.Empty;
        }
    }
}
