using UnityEngine;
using NodeEditorFramework;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gameflow
{
    [Node(false, "Event/Set Camera", new System.Type[] { typeof(DialogCanvas) })]
    public class SetCameraNode : BaseNodeIO
    {
        public override string Title => "Set Camera";
        public override Vector2 MinSize => new Vector2(150, 40);

        public const string ID = "setCamera";
        public override string GetID => ID;
        public override bool ExecuteDuringLoading => true;

        public Vector2 Pivot = Vector2.one/2;

        private Bounds pivotBounds = new Bounds(Vector2.one/2, Vector2.one);

#if UNITY_EDITOR
        public override void NodeGUI()
        {
            GUILayout.BeginVertical("box");
            Pivot = pivotBounds.ClosestPoint(EditorGUILayout.Vector2Field("Pivot", Pivot));
            GUILayout.EndVertical();
        }
#endif
    }
}
