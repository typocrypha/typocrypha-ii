using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace Gameflow
{
    [Node(false, "Battle/Victory Screen Unlock Message", new System.Type[] { typeof(BattleCanvas) })]
    public class VictoryScreenUnlockNode : BaseNodeIO
    {

        public override Vector2 MinSize => new Vector2(250, 80);

        new public const string ID = "Victory Screen Unlock Message";
        public override string GetID => ID;
        public override string Title => "Unlock Message";

        public string unlockedText;
        public string reasonText;
        public string descriptionText;
        public int priority;
        public string clarkeMessage;
        public Sprite iconSprite;

        private const string priorityTooltip = "public const int criticalPriority = 100; " +
         "\npublic const int criticalPriority2 = 99;" +
         "\npublic const int criticalPriority3 = 98;" +
         "\npublic const int highPriority = 50;" +
         "\npublic const int highPriority2 = 49;" +
         "\npublic const int highPriority3 = 48;" +
         "\npublic const int lowPriority = 3;" +
         "\npublic const int lowPriority2 = 2;" +
         "\npublic const int lowPriority3 = 1;";

        public override void NodeGUI()
        {
            GUILayout.Space(4);
            GUILayout.BeginVertical("Box");

            unlockedText = RTEditorGUI.TextField(new GUIContent("Unlocked Text"), unlockedText);
            GUILayout.Label("Reason Text");
            GUIStyle dialogTextStyle = new GUIStyle(GUI.skin.textArea);
            dialogTextStyle.wordWrap = true;
            reasonText = GUILayout.TextArea(reasonText, GUI.skin.textArea, GUILayout.MinHeight(RTEditorGUI.lineHeight * 2));
            GUILayout.Label("Description Text");
            descriptionText = GUILayout.TextArea(descriptionText, GUI.skin.textArea, GUILayout.MinHeight(RTEditorGUI.lineHeight * 2));
            GUILayout.Label("Clarke Message");
            clarkeMessage = GUILayout.TextArea(clarkeMessage, GUI.skin.textArea, GUILayout.MinHeight(RTEditorGUI.lineHeight * 2));
            priority = RTEditorGUI.IntField(new GUIContent("Priority", priorityTooltip), priority);
            iconSprite = RTEditorGUI.ObjectField("Icon Sprite", iconSprite, false);
            GUILayout.EndVertical();

            //Don't know why this code needs to be here exactly, but it makes everything nicer? maybe add to some static stuff?
            GUILayout.BeginHorizontal();
            GUILayout.EndHorizontal();
        }
    }
}
