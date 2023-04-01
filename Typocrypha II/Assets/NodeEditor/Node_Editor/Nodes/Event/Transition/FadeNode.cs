using System.Collections;
using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace Gameflow
{
    [Node(false, "Event/Transition/Fade", new System.Type[] { typeof(DialogCanvas) })]
    public class FadeNode : BaseNodeIO, ITimedNode {
        public enum FadeType
        {
            Fade_In,
            Fade_Out,
        }

        #region ITimedNode
        static bool isCompleted;
        public bool IsCompleted // Wait for fade to complete.
        {
            get => isCompleted;
        }
        #endregion

        /// <summary>
        /// Fade the entire screen (coroutine).
        /// </summary>
        /// <param name="fadeTime">Amount of time to complete fade.</param>
        /// <param name="fadeStart">Starting amount of normlized fade.</param>
        /// <param name="fadeEnd">End amount of normalized fade</param>
        /// <param name="fadeColor">Color of fade.</param>
        public static IEnumerator FadeScreenOverTime(float fadeTime, float fadeStart, float fadeEnd, Color fadeColor)
        {
            isCompleted = false;
            yield return FaderManager.instance.FadeScreenOverTime(fadeTime, fadeStart, fadeEnd, fadeColor);
            isCompleted = true;
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
            fadeColor = RTEditorGUI.ColorField(fadeColor);
            GUILayout.EndVertical();
            if (fadeType == FadeType.Fade_In)
                _title = "Fade In";
            else
                _title = "Fade Out";
        }
        #endregion
    }
}
