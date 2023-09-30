using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace Gameflow
{
    [Node(false, "Dialog/Embed Image", new System.Type[] { typeof(DialogCanvas) })]
    public class EmbedImage : BaseNodeIO
    {
        public const string ID = "Embed Image Node";
        public override string GetID => ID;

        public override string Title { get { return "Embed Image"; } }
        public override Vector2 MinSize { get { return new Vector2(100, 60); } }

        public Sprite sprite;

#if UNITY_EDITOR
        public override void NodeGUI()
        {
            GUILayout.BeginVertical(new GUIStyle());
            sprite = RTEditorGUI.ObjectField(sprite, false);
            GUILayout.EndVertical();
        }
#endif
    }
}