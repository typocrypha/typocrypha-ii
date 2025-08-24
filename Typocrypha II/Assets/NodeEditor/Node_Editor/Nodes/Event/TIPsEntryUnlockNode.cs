using NodeEditorFramework;
using NodeEditorFramework.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameflow
{
    [Node(false, "Event/Unlock TIPs Entry", typeof(BattleCanvas), typeof(DialogCanvas))]
    public class TIPsEntryUnlockNode : BaseNodeIO
    {
        public const string Id = "TIPsEntryUnlockNode";
        public override string GetID => Id;

        public override string Title => "Unlock TIPs Entry";
        public override Vector2 MinSize => new Vector2(150, 40);

        public IEnumerable<string> Entries
        {
            get
            {
                yield return entry1;
                yield return entry2;
                yield return entry3;
            }
        }

        public string entry1;
        public string entry2;
        public string entry3;

        public override void NodeGUI()
        {
            GUILayout.Space(4);
            GUILayout.BeginVertical("Box");

            GUILayout.Label(new GUIContent("Entries"), GUI.skin.label);
            entry1 = RTEditorGUI.TextField(entry1);
            entry2 = RTEditorGUI.TextField(entry2);
            entry3 = RTEditorGUI.TextField(entry3);

            //Don't know why this code needs to be here exactly, but it makes everything nicer? maybe add to some static stuff?
            GUILayout.BeginHorizontal();
            GUILayout.EndHorizontal();
        }
    }
}
