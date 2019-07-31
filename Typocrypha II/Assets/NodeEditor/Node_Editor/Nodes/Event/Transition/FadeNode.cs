using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace Gameflow {
    [Node(false, "Event/Transition/Fade", new System.Type[] { typeof(GameflowCanvas), typeof(DialogCanvas) })]
    public class FadeNode : BaseNodeIO {
        public enum FadeType
        {
            Fade_In,
            Fade_Out,
        }

        #region Editor
        public const string ID = "Fade Transition Node";
        public override string GetID { get { return ID; } }

        string _title;
        public override string Title { get { return _title; } }
        public override Vector2 MinSize { get { return new Vector2(150, 60); } }
        public override bool AutoLayout { get { return true; } }

        public FadeType fadeType;
        public float fadeTime;
        public Color fadeColor;

        const string tooltip_fadeTime = "The time it will take for the fade to complete. Player control will be automatically disabled for the duration.";

        protected override void OnCreate()
        {
            _title = "Fade";
            fadeType = FadeType.Fade_In;
            fadeTime = 3f;
            fadeColor = Color.black;
        }

        public override void NodeGUI()
        {
            GUILayout.Space(5);
            GUILayout.BeginVertical("box");
            fadeType = (FadeType)RTEditorGUI.EnumPopup(fadeType);
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Fade Time", tooltip_fadeTime), NodeEditorGUI.nodeLabel, GUILayout.Width(65f));
            GUILayout.Space(3);
            fadeTime = RTEditorGUI.FloatField(fadeTime, GUILayout.Width(72));
            GUILayout.EndHorizontal();
            GUILayout.Space(3);
            //fadeColor = RTEditorGUI..ColorField(fadeColor);
            GUILayout.EndVertical();
            if (fadeType == FadeType.Fade_In)
                _title = "Fade In";
            else
                _title = "Fade Out";
        }
        #endregion
    }
}
