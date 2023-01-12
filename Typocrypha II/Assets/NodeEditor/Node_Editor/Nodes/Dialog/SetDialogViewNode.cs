using UnityEngine;
using NodeEditorFramework;
using System.Collections.Generic;

namespace Gameflow
{

    [Node(false, "Dialog/Set Dialog View", new System.Type[] { typeof(GameflowCanvas), typeof(DialogCanvas) })]
    public class SetDialogViewNode : BaseNodeIO
    {
        public override string Title { get { return "Set Dialog View"; } }
        public override Vector2 MinSize { get { return new Vector2(150, 50); } }
        public override bool AutoLayout { get { return true; } }

        public const string Id = "Set Dialog View Node";
        public override string GetID { get { return Id; } }

        public System.Type ViewType => viewMap.ContainsKey(viewName) ? viewMap[viewName] : null;
        public string viewName;

        // Dialog view labels.
        public static Dictionary<string, System.Type> viewMap = new Dictionary<string, System.Type>
        {
            {"vn", typeof(DialogViewVN) },
            {"chat", typeof(DialogViewChat) },
            {"an", typeof(DialogViewAN) },
            {"bubble", typeof(DialogViewBubble) },
            {"input", typeof(DialogNodeInput) },
            {"vnplus", typeof(DialogViewVNPlus) }
        };

        public override void NodeGUI()
        {
            GUILayout.Space(5);
            GUILayout.BeginVertical("box");
            GUILayout.Label(new GUIContent($"View Name:"), NodeEditorGUI.nodeLabelBoldCentered);
            viewName = GUILayout.TextField(viewName);
            GUILayout.EndVertical();
        }

        protected override void OnCreate()
        {
            viewName = "vnplus";
        }
    }
}
