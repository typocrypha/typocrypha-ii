using NodeEditorFramework;
using NodeEditorFramework.Utilities;
using UnityEngine;

namespace Gameflow
{
    [Node(false, "Event/Fade Screen", new System.Type[] { typeof(DialogCanvas) })]
    public class FadeNode : BaseNodeIO
    {
        public enum FadeType
        {
            FadeIn,
            FadeOut,
        }

        #region Editor
        public const string ID = "Fade Transition Node";
        public override string GetID => ID;
        public override string Title => fadeType == FadeType.FadeIn ? "Fade In" : "Fade Out";
        public override Vector2 MinSize => new Vector2(150, 60);
        public override bool AutoLayout => true;

        public FadeType fadeType;
        public float fadeTime;
        public Color fadeColor;

        const string tooltip_fadeTime = "The time it will take for the fade to complete. Player control will be automatically disabled for the duration.";

        protected override void OnCreate()
        {
            fadeType = FadeType.FadeIn;
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
        }
        #endregion
    }
}
