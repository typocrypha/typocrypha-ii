using UnityEngine;
using NodeEditorFramework;
using UnityEditor;

namespace Gameflow
{
    [Node(false, "Event/Audio/Crossfade BGM", new System.Type[] { typeof(DialogCanvas) })]
    public class CrossfadeBgm : AudioControlNode
    {
        public const string ID = "Crossfade BGM Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Crossfade BGM"; } }
        public override Vector2 MinSize { get { return new Vector2(250, 60); } }
        
        public AudioClip bgm;
        public AnimationCurve fadeCurveIn = AnimationCurve.Linear(0,0,1,1);
        public AnimationCurve fadeCurveOut = AnimationCurve.Linear(0,1,1,0);

        #region Tooltip Strings
        const string tooltipBgm = "AudioClip to fade in as bgm.";
        const string tooltipFadeIn = "Volume curve over which new audio clip fades in.";
        const string tooltipFadeOut = "Volume curve over which old audio clip fades out.";
        #endregion

        private const float CURVE_HEIGHT = 75f;

        public override void NodeGUI()
        {
            GUILayout.BeginVertical(new GUIStyle()); //BEGIN INSPECTOR

            #region BGM
            GUILayout.BeginVertical("Box");
            EditorGUIUtility.labelWidth = 65f;
            bgm = EditorGUILayout.ObjectField(new GUIContent("BGM", tooltipBgm), bgm, typeof(AudioClip), false) as AudioClip;
            GUILayout.EndVertical();
            #endregion

            GUILayout.BeginHorizontal(); //BEGIN CURVES

            #region FadeIn
            if(fadeCurveIn != null)
            {
                GUILayout.BeginVertical();
                GUILayout.Label(new GUIContent("Fade In Curve", tooltipFadeIn), NodeEditorGUI.nodeLabelBoldCentered);
                fadeCurveIn = EditorGUILayout.CurveField(fadeCurveIn, GUILayout.Height(CURVE_HEIGHT));
                GUILayout.EndVertical();
            }
            #endregion

            #region FadeOut
            if(fadeCurveOut != null)
            {
                GUILayout.BeginVertical();
                GUILayout.Label(new GUIContent("Fade Out Curve", tooltipFadeOut), NodeEditorGUI.nodeLabelBoldCentered);
                fadeCurveOut = EditorGUILayout.CurveField(fadeCurveOut, GUILayout.Height(CURVE_HEIGHT));
                GUILayout.EndVertical();
            }
            #endregion

            GUILayout.EndHorizontal(); //END CURVES

            GUILayout.EndVertical(); //END INSPECTOR
        }
    }
}
