using UnityEngine;
using NodeEditorFramework;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gameflow
{
    [Node(false, "Event/Audio/Play BGM", new System.Type[] { typeof(DialogCanvas) })]
    public class PlayBgm : AudioControlNode
    {
        public const string ID = "Play BGM Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Play BGM"; } }
        public override Vector2 MinSize { get { return new Vector2(250, 60); } }

        public AudioClip bgm;
        public AnimationCurve fadeCurve = AnimationCurve.Linear(0,0,1,1);

        #region Tooltip Strings
        const string tooltipBgm = "AudioClip to play as bgm.";
        const string tooltipFade = "Volume curve over which audio clip fades in.";
        #endregion

        private const float CURVE_HEIGHT = 75f;
#if UNITY_EDITOR
        public override void NodeGUI()
        {
            GUILayout.BeginVertical(new GUIStyle());

            #region BGM
            GUILayout.BeginVertical("Box");
            EditorGUIUtility.labelWidth = 65f;
            bgm = EditorGUILayout.ObjectField(new GUIContent("BGM", tooltipBgm), bgm, typeof(AudioClip), false) as AudioClip;
            GUILayout.EndVertical();
            #endregion

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
#endif
    }
}