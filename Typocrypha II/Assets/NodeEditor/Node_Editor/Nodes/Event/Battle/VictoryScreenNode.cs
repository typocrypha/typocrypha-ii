﻿using System.Linq;
//using System.Collections;
//using System.Collections.Generic;
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
        public override Vector2 MinSize { get { return new Vector2(240, 100); } }

        public const string Id = "victoryScreen";
        public override string GetID { get { return Id; } }

        const int MAX_ENTRIES = 8;
        [SerializeField] TallyEntry[] entries = new TallyEntry[MAX_ENTRIES];
        public int numEntries = 1;
        public TallyEntry[] Entries => entries.Take(numEntries).ToArray();
        public string ClarkeText = "";

        public int Total
        {
            get
            {
                float total = 0, percentMultiplier = 0;
                for (int i = 0; i < numEntries; ++i)
                {
                    if (!entries[i].isPercentage) total += entries[i].value;
                    else percentMultiplier += entries[i].value;
                }
                total *= 1 + percentMultiplier/100;
                return Mathf.FloorToInt(total);
            }
        }

#if UNITY_EDITOR
        public override void NodeGUI()
        {
            GUILayout.BeginVertical("box");
            numEntries = Mathf.Clamp(EditorGUILayout.IntField("Number of Tallies", numEntries), 0, MAX_ENTRIES);

            GUI.enabled = false;
            GUILayout.BeginHorizontal();
            RTEditorGUI.TextField("Label", GUILayout.ExpandWidth(true));
            RTEditorGUI.TextField("Value", GUILayout.Width(50));
            RTEditorGUI.TextField("%", GUILayout.Width(21));
            GUILayout.EndHorizontal();
            GUI.enabled = true;

            for (int i = 0; i < numEntries; ++i)
            {
                GUILayout.BeginHorizontal();
                var label = RTEditorGUI.TextField(entries[i].label, GUILayout.ExpandWidth(true));
                var value = RTEditorGUI.IntField(entries[i].value, GUILayout.Width(50));
                var percent = RTEditorGUI.Toggle(entries[i].isPercentage, "", GUILayout.Width(16));
                entries[i] = new TallyEntry { label = label, value = value, isPercentage = percent };
                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            GUI.enabled = false;
            RTEditorGUI.TextField("Total", GUILayout.ExpandWidth(true));
            RTEditorGUI.IntField(Total, GUILayout.Width(71));
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.Label(new GUIContent("Clarke Text"));
            ClarkeText = GUILayout.TextArea(ClarkeText, GUI.skin.textArea, GUILayout.MinHeight(RTEditorGUI.lineHeight * 2));

            GUILayout.EndVertical();
        }
    }
#endif
}
