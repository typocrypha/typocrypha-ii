using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

using System;
using Gameflow.GUIUtilities;

namespace Gameflow
{
    [Node(false, "Event/Audio/Play BGM", new System.Type[] { typeof(GameflowCanvas), typeof(DialogCanvas) })]
    public class PlayBgm : AudioControlNode
    {
        public const string ID = "Play BGM Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Play BGM"; } }
        public override Vector2 MinSize { get { return new Vector2(250, 60); } }

        public AudioClip bgm;
        public AnimationCurve fadeCurve;

        #region Tooltip Strings
        const string tooltipBgm = "AudioClip to play as bgm.";
        const string tooltipFade = "Volume curve over which audio clip fades in.";
        #endregion

        public override void NodeGUI()
        {
            #region BGM
            GUILayout.Space(5);
            GUILayout.BeginVertical("Box");
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("BGM", tooltipBgm), NodeEditorGUI.nodeLabel, GUILayout.Width(65f));
            bgm = RTEditorGUI.ObjectField(bgm, false, GUILayout.Width(MinSize.x - 100f));
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            #endregion

            #region FadeIn
            if(fadeCurve != null)
            {
                GUILayout.Label(new GUIContent("Fade In Curve", tooltipFade), NodeEditorGUI.nodeLabelBoldCentered);
                GUILayout.BeginVertical();
                GUILayout.Space(100);
                fadeCurve = RTEditorGUI.CurveField(new Rect(4, 50, MinSize.x - 10, 100), fadeCurve);
                GUILayout.EndVertical();
            }
            #endregion
        }
    }
}
