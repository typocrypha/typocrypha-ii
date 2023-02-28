using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace Gameflow
{
    [Node(false, "Event/Audio/Play SFX", new System.Type[] { typeof(GameflowCanvas), typeof(DialogCanvas) })]
    public class PlaySfx : AudioControlNode
    {
        public const string ID = "Play SFX Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Play SFX"; } }
        public override Vector2 MinSize { get { return new Vector2(250, 60); } }

        public AudioClip sfx;

        #region Tooltip Strings
        const string tooltipSfx = "AudioClip to play as sfx.";
        #endregion

        public override void NodeGUI()
        {
            #region SFX
            GUILayout.Space(5);
            GUILayout.BeginVertical("Box");
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("SFX", tooltipSfx), NodeEditorGUI.nodeLabel, GUILayout.Width(65f));
            sfx = RTEditorGUI.ObjectField(sfx, false, GUILayout.Width(MinSize.x - 100f));
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            #endregion
        }
    }
}