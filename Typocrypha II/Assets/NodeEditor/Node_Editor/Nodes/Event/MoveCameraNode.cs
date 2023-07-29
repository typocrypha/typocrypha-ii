using UnityEngine;
using NodeEditorFramework;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gameflow
{
    [Node(false, "Event/Move Camera", new System.Type[] { typeof(DialogCanvas) })]
    public class MoveCameraNode : BaseNodeIO
    {
        public override string Title { get { return "Move Camera"; } }
        public override Vector2 MinSize { get { return new Vector2(150, 40); } }

        public const string ID = "moveCamera";
        public override string GetID { get { return ID; } }

        public Vector2 StartPivot = Vector2.one/2;
        public Vector2 FinalPivot = Vector2.one/2;
        public float Duration = 1f;
        public AnimationCurve EasingCurve = AnimationCurve.Linear(0,0,1,1);

        private Bounds pivotBounds = new Bounds(Vector2.one/2, Vector2.one);

        #region Tooltip Strings
        const string tooltipEasingCurve = "Easing curve used by the camera tween.";
        #endregion

        private const float CURVE_HEIGHT = 75f;

#if UNITY_EDITOR
        public override void NodeGUI()
        {
            GUILayout.BeginVertical("box");
            StartPivot = pivotBounds.ClosestPoint(EditorGUILayout.Vector2Field("StartPivot", StartPivot));
            FinalPivot = pivotBounds.ClosestPoint(EditorGUILayout.Vector2Field("FinalPivot", FinalPivot));
            Duration = EditorGUILayout.FloatField("Duration", Duration);
            GUILayout.EndVertical();
            GUILayout.Label(new GUIContent("Easing Curve", tooltipEasingCurve), NodeEditorGUI.nodeLabelBoldCentered);
            EasingCurve = EditorGUILayout.CurveField(EasingCurve, GUILayout.Height(CURVE_HEIGHT));
        }
#endif
    }
}
