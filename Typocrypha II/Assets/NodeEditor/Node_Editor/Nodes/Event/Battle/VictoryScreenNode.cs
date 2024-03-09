using System.Linq;
//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;
using TallyEntry = VictoryResultsScreen.TallyEntry;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gameflow
{
    [Node(false, "Battle/Victory Screen", typeof(BattleCanvas))]
    public class VictoryScreenNode : BaseNodeIO
    {
        public override string Title { get { return "Victory Screen"; } }
        public override Vector2 MinSize { get { return new Vector2(200, 100); } }

        public const string Id = "victoryScreen";
        public override string GetID { get { return Id; } }

        const int MAX_ENTRIES = 8;
        [SerializeField] TallyEntry[] entries = new TallyEntry[MAX_ENTRIES];
        public int numEntries = 1;
        public TallyEntry[] Entries => entries.Take(numEntries).ToArray();
        public int Total
        {
            get
            {
                int total = 0;
                for (int i = 0; i < numEntries; ++i) total += entries[i].value;
                return total;
            }
        }

#if UNITY_EDITOR
        public override void NodeGUI()
        {
            GUILayout.BeginVertical("box");
            numEntries = Mathf.Clamp(EditorGUILayout.IntField("Number of Tallies", numEntries), 0, MAX_ENTRIES);
            for (int i = 0; i < numEntries; ++i)
            {
                GUILayout.BeginHorizontal();
                var label = RTEditorGUI.TextField(entries[i].label, GUILayout.ExpandWidth(true));
                var value = RTEditorGUI.IntField(entries[i].value, GUILayout.Width(50));
                entries[i] = new TallyEntry { label = label, value = value };
                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            GUI.enabled = false;
            RTEditorGUI.TextField("Total", GUILayout.ExpandWidth(true));
            RTEditorGUI.IntField(Total, GUILayout.Width(50));
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }
    }
#endif
}
