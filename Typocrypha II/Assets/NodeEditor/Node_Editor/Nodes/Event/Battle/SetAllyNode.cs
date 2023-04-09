using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace Gameflow
{
    [Node(false, "Event/Set Ally", new System.Type[] { typeof(BattleCanvas), typeof(DialogCanvas) })]
    public class SetAllyNode : BaseNodeIO
    {
        public override Vector2 MinSize => new Vector2(250, 100);

        public const string ID = "Set Ally Node";
        public override string GetID => ID;
        public override string Title => "Set Ally";

        public CharacterData allyData;
        public string pose;
        public string expr;
        public GameObject prefab;
        public bool show;

        public override void NodeGUI()
        {
            GUILayout.Space(4);
            GUILayout.BeginVertical("Box");
            GUILayout.BeginHorizontal();
            GUILayout.Label("Caster Prefab", GUILayout.Width(100));
            prefab = RTEditorGUI.ObjectField(prefab, false, GUILayout.Width(125));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Character Data", GUILayout.Width(100));
            allyData = RTEditorGUI.ObjectField(allyData, false, GUILayout.Width(125));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Initial Expr"), GUILayout.Width(100));
            expr = RTEditorGUI.TextField(expr);
            GUILayout.Space(5);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Initial Pose"), GUILayout.Width(100));
            pose = RTEditorGUI.TextField(pose);
            GUILayout.Space(5);
            GUILayout.EndHorizontal();
            show = GUILayout.Toggle(show, new GUIContent("Show Immediately"));

            GUILayout.EndVertical();

            //Don't know why this code needs to be here exactly, but it makes everything nicer? maybe add to some static stuff?
            GUILayout.BeginHorizontal();
            GUILayout.EndHorizontal();
        }
    }
}
