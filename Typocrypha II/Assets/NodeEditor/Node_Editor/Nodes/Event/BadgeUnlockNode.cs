using NodeEditorFramework;
using NodeEditorFramework.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameflow
{
    [Node(false, "Event/Badge Unlock", typeof(BattleCanvas), typeof(DialogCanvas))]
    public class BadgeUnlockNode : BaseNodeIO
    {
        private const string id = "BadgeUnlockNode";
        public override string GetID => id;

        public override string Title => "Badge Unlock";
        public override Vector2 MinSize => new Vector2(150, 40);

        public IEnumerable<BadgeWord> Words
        {
            get
            {
                yield return word1;
                yield return word2;
                yield return word3;
            }
        }

        public BadgeWord word1;
        public BadgeWord word2;
        public BadgeWord word3;

        public override void NodeGUI()
        {
            GUILayout.Space(4);
            GUILayout.BeginVertical("Box");

            GUILayout.Label(new GUIContent("Badges"), GUI.skin.label);
            word1 = RTEditorGUI.ObjectField(word1, false);
            word2 = RTEditorGUI.ObjectField(word2, false);
            word3 = RTEditorGUI.ObjectField(word3, false);

            //Don't know why this code needs to be here exactly, but it makes everything nicer? maybe add to some static stuff?
            GUILayout.BeginHorizontal();
            GUILayout.EndHorizontal();
        }
    }
}
