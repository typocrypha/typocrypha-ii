using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace Gameflow
{
    [Node(false, "Event/Cast Spell", new System.Type[] { typeof(DialogCanvas) })]
    public class CastSpellNode : BaseNodeIO
    {
        #region Editor
        public override string Title { get { return "Cast Spell"; } }
        public override Vector2 MinSize { get { return new Vector2(300, 40); } }

        public const string Id = "castSpell";
        public override string GetID { get { return Id; } }

        public SpellWord word1;
        public SpellWord word2;
        public SpellWord word3;
        public Vector2Int targetPos;
        public Vector2Int casterPos;
        public string messageOverride;
        public string proxyCasterName;

        public Spell GetSpell()
        {
            var spell = new Spell();
            if(word1 != null)
            {
                spell.Add(word1);
            }
            if (word2 != null)
            {
                spell.Add(word2);
            }
            if (word3 != null)
            {
                spell.Add(word3);
            }
            return spell;
        }

        protected override void OnCreate()
        {

        }

        public override void NodeGUI()
        {
            GUILayout.BeginVertical("box");

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Spell"), GUI.skin.label, GUILayout.Width(40));
            word1 = RTEditorGUI.ObjectField(word1, false, GUILayout.Width(80));
            word2 = RTEditorGUI.ObjectField(word2, false, GUILayout.Width(80));
            word3 = RTEditorGUI.ObjectField(word3, false, GUILayout.Width(80));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Target Pos"), GUI.skin.label, GUILayout.Width(70));
            targetPos = RTEditorGUI.Vector2IntField("", targetPos);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Caster Pos"), GUI.skin.label, GUILayout.Width(70));
            casterPos = RTEditorGUI.Vector2IntField("", casterPos);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Proxy Caster"), GUI.skin.label, GUILayout.Width(80));
            proxyCasterName = GUILayout.TextField(proxyCasterName);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Msg Override"), GUI.skin.label, GUILayout.Width(80));
            messageOverride = GUILayout.TextField(messageOverride);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }
        #endregion
    }
}
