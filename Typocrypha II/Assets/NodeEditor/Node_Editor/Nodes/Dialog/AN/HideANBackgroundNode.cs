using NodeEditorFramework;
using NodeEditorFramework.Utilities;
using UnityEngine;

namespace Gameflow
{
    [Node(false, "Event/Hide AN BG", new System.Type[] { typeof(DialogCanvas) })]
    public class HideANBackgroundNode : BaseNodeIO
    {
        public const string ID = "Hide AN BG";
        public override string GetID => ID;

        public override Vector2 MinSize => new Vector2(150, 60);
        public override bool AutoLayout => true;

        public float fadeTime = 3f;

        public override void NodeGUI()
        {
            GUILayout.Space(5);
            GUILayout.BeginVertical("box");
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Fade Time"), NodeEditorGUI.nodeLabel, GUILayout.Width(65f));
            GUILayout.Space(3);
            fadeTime = RTEditorGUI.FloatField(fadeTime, GUILayout.Width(72));
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
    }
}
