using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace Gameflow
{
    [Node(false, "Event/Add Spell(s)", new System.Type[] { typeof(GameflowCanvas), typeof(DialogCanvas) })]
    public class AddEquippedSpellsNode : BaseNodeIO
    {
        #region Editor
        public override string Title { get { return "Add Spell(s)"; } }
        public override Vector2 MinSize { get { return new Vector2(150, 40); } }

        public const string Id = "addEquippedSpells";
        public override string GetID { get { return Id; } }

        public SpellWord word1;
        public SpellWord word2;
        public SpellWord word3;

        public override void NodeGUI()
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label(new GUIContent("Spell Words"), GUI.skin.label);
            word1 = RTEditorGUI.ObjectField(word1, false);
            word2 = RTEditorGUI.ObjectField(word2, false);
            word3 = RTEditorGUI.ObjectField(word3, false);
            GUILayout.EndVertical();
        }
        #endregion
    }
}
