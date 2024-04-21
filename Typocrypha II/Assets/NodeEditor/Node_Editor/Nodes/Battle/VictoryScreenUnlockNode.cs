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

        public override void NodeGUI()
        {
            GUILayout.Space(4);
            GUILayout.BeginVertical("Box");

            unlockedText = RTEditorGUI.TextField(new GUIContent("Unlocked Text"), unlockedText);
            GUILayout.Label("Reason Text");
            GUIStyle dialogTextStyle = new GUIStyle(GUI.skin.textArea);
            dialogTextStyle.wordWrap = true;
            reasonText = GUILayout.TextArea(reasonText, GUI.skin.textArea, GUILayout.MinHeight(RTEditorGUI.lineHeight * 2));

            GUILayout.EndVertical();

            //Don't know why this code needs to be here exactly, but it makes everything nicer? maybe add to some static stuff?
            GUILayout.BeginHorizontal();
            GUILayout.EndHorizontal();
        }
    }
}
