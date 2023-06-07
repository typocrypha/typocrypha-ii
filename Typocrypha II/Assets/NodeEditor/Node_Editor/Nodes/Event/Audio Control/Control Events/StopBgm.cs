using UnityEngine;
using NodeEditorFramework;
using UnityEditor;

namespace Gameflow
{
    [Node(false, "Event/Audio/Stop BGM", new System.Type[] { typeof(DialogCanvas) })]
    public class StopBgm : AudioControlNode
    {
        public const string ID = "Stop BGM Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Stop BGM"; } }
        public override Vector2 MinSize { get { return new Vector2(250, 60); } }
        
        public AnimationCurve fadeCurve = AnimationCurve.Linear(0,1,1,0);

        #region Tooltip Strings
        const string tooltipFade = "Volume curve over which audio clip fades out.";
        #endregion

        private const float CURVE_HEIGHT = 75f;

        public override void NodeGUI()
        {
            GUILayout.BeginVertical(new GUIStyle());

            #region FadeIn
            if(fadeCurve != null)
            {
                GUILayout.BeginVertical();
                GUILayout.Label(new GUIContent("Fade Curve", tooltipFade), NodeEditorGUI.nodeLabelBoldCentered);
                fadeCurve = EditorGUILayout.CurveField(fadeCurve, GUILayout.Height(CURVE_HEIGHT));
                GUILayout.EndVertical();
            }
            #endregion

            GUILayout.EndVertical();
        }
    }
}
