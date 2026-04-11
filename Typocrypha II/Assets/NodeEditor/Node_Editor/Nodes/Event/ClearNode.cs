using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace Gameflow
{
    [Node(false, "Event/Clear", new System.Type[] { typeof(DialogCanvas) })]
    public class ClearNode : BaseNodeIO
    {
        #region Editor
        public override string Title => "Clear";
        public override Vector2 MinSize => new Vector2(150, 40);

        public const string ID = "clear";
        public override string GetID => ID;

        public bool instant = false;
        #endregion

        public override void NodeGUI()
        {
            GUILayout.Space(5);
            GUILayout.BeginVertical("box");
            instant = RTEditorGUI.Toggle(instant, new GUIContent("Instant"));
            GUILayout.EndVertical();
        }
    }
}
