using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace Gameflow
{
    [Node(false, "Event/Audio/Stop BGM", new System.Type[] { typeof(GameflowCanvas), typeof(DialogCanvas) })]
    public class StopBgm : AudioControlNode
    {
        public const string ID = "Stop BGM Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Stop BGM"; } }
        public override Vector2 MinSize { get { return new Vector2(250, 60); } }
        
        public AnimationCurve fadeCurve;

        #region Tooltip Strings
        const string tooltipFade = "Volume curve over which audio clip fades out.";
        #endregion

        public override void NodeGUI()
        {
            #region FadeIn
            if(fadeCurve != null)
            {
                GUILayout.Label(new GUIContent("Fade Out Curve", tooltipFade), NodeEditorGUI.nodeLabelBoldCentered);
                GUILayout.BeginVertical();
                GUILayout.Space(100);
                fadeCurve = RTEditorGUI.CurveField(new Rect(4, 15, MinSize.x - 10, 100), fadeCurve);
                GUILayout.EndVertical();
            }
            #endregion
        }
    }
}
