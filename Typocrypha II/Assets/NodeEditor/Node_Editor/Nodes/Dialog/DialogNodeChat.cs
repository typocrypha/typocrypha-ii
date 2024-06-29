using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace Gameflow
{
    [Node(false, "Dialog/Chat Dialog", new System.Type[] { typeof(DialogCanvas) })]
    public class DialogNodeChat : DialogNode
    {
        #region Editor
        public const string ID = "Chat Dialog Node";
        public override string GetID => ID;

        public override string Title => "Chat Dialog";
        public override Vector2 MinSize => new Vector2(250, 60);

        public IconSide iconSide;
        public string timeText;

        #region Tooltip Strings
        private const string tooltipSprite = "Sprites on the left and right side of the chat box.";
        #endregion

        protected override void OnCreate()
        {
            base.OnCreate();
            characterName = "Name (optional)";
        }

        public override void NodeGUI()
        {
            NameGUI();
            TextGUI();
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Time Text"), NodeEditorGUI.nodeLabel, GUILayout.Width(65f));
            timeText = GUILayout.TextField(timeText, GUILayout.Width(MinSize.x - 85));
            GUILayout.EndHorizontal();

            //Don't know why this code needs to be here exactly, but it makes everything nicer? maybe add to some static stuff?
            GUILayout.BeginHorizontal();
            RTEditorGUI.labelWidth = 90;
            GUILayout.EndHorizontal();

            #region Icon Sprites
            GUILayout.BeginVertical("Box");
            iconSide = (IconSide)RTEditorGUI.EnumPopup(iconSide);
            GUILayout.EndVertical();
            #endregion
        }
        #endregion
    }

}
