using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SerializableCollections.GUIUtils;
using System.Linq;

public static partial class EditorUtils
{
    public static class CasterUtils
    {
        public static void CasterStatsGUILayout(CasterStats data)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Max HP", "TODO: tooltip"), GUILayout.Width(75));
            data.MaxHP = EditorGUILayout.IntField(data.MaxHP, GUILayout.Width(47.5f));
            EditorGUILayout.LabelField(new GUIContent("Max SP", "TODO: tooltip"), GUILayout.Width(75));
            data.MaxSP = EditorGUILayout.IntField(data.MaxSP, GUILayout.Width(47.5f));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Max Armor", "TODO: tooltip"), GUILayout.Width(75));
            data.MaxArmor = EditorGUILayout.IntField(data.MaxArmor, GUILayout.Width(47.5f));
            EditorGUILayout.LabelField(new GUIContent("Max Stagger", "TODO: tooltip"), GUILayout.Width(75));
            data.MaxStagger = EditorGUILayout.IntField(data.MaxStagger, GUILayout.Width(47.5f));
            EditorGUILayout.EndHorizontal();
            data.StaggerTime = EditorGUILayout.FloatField(new GUIContent("Stagger Time", "TODO: tooltip"), data.StaggerTime);
            data.Atk = EditorGUILayout.IntSlider(new GUIContent("Atk", "TODO: tooltip"), data.Atk, CasterStats.statMin, CasterStats.statMax);
            data.Def = EditorGUILayout.IntSlider(new GUIContent("Def", "TODO: tooltip"), data.Def, CasterStats.statMin, CasterStats.statMax);
            data.Spd = EditorGUILayout.IntSlider(new GUIContent("Speed", "TODO: tooltip"), data.Spd, CasterStats.statMin, CasterStats.statMax);
            data.Acc = EditorGUILayout.IntSlider(new GUIContent("Acc", "TODO: tooltip"), data.Acc, CasterStats.statMin, CasterStats.statMax);
            data.Evade = EditorGUILayout.IntSlider(new GUIContent("Evade", "TODO: tooltip"), data.Evade, CasterStats.statMin, CasterStats.statMax);
            data.Luck = EditorGUILayout.IntSlider(new GUIContent("Luck", "TODO: tooltip"), data.Luck, CasterStats.statMin, CasterStats.statMax);
        }
        public static void CasterStatsLabelLayout(CasterStats data)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Max HP:", "TODO: tooltip"), GUILayout.Width(75));
            EditorGUILayout.LabelField(new GUIContent(data.MaxHP.ToString(), "TODO: tooltip"), Bold, GUILayout.Width(50));
            EditorGUILayout.LabelField(new GUIContent("Max SP:", "TODO: tooltip"), GUILayout.Width(75));
            EditorGUILayout.LabelField(new GUIContent(data.MaxSP.ToString(), "TODO: tooltip"), Bold, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Max Armor:", "TODO: tooltip"), GUILayout.Width(75));
            EditorGUILayout.LabelField(new GUIContent(data.MaxArmor.ToString(), "TODO: tooltip"), Bold, GUILayout.Width(50));
            EditorGUILayout.LabelField(new GUIContent("Max Stagger:", "TODO: tooltip"), GUILayout.Width(75));
            EditorGUILayout.LabelField(new GUIContent(data.MaxStagger.ToString(), "TODO: tooltip"), Bold, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Stun Time:", "TODO: tooltip"), GUILayout.Width(75));
            EditorGUILayout.LabelField(new GUIContent(data.StaggerTime.ToString(), "TODO: tooltip"), Bold, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Atk:", "TODO: tooltip"), GUILayout.Width(50));
            EditorGUILayout.LabelField(new GUIContent(data.Atk.ToString(), "TODO: tooltip"),Bold, GUILayout.Width(31));
            EditorGUILayout.LabelField(new GUIContent("Def:", "TODO: tooltip"), GUILayout.Width(50));
            EditorGUILayout.LabelField(new GUIContent(data.Def.ToString(), "TODO: tooltip"), Bold, GUILayout.Width(31));
            EditorGUILayout.LabelField(new GUIContent("Speed:", "TODO: tooltip"), GUILayout.Width(50));
            EditorGUILayout.LabelField(new GUIContent(data.Spd.ToString(), "TODO: tooltip"), Bold, GUILayout.Width(31));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Acc:", "TODO: tooltip"), GUILayout.Width(50));
            EditorGUILayout.LabelField(new GUIContent(data.Acc.ToString(), "TODO: tooltip"), Bold, GUILayout.Width(31));
            EditorGUILayout.LabelField(new GUIContent("Evade:", "TODO: tooltip"), GUILayout.Width(50));
            EditorGUILayout.LabelField(new GUIContent(data.Evade.ToString(), "TODO: tooltip"), Bold, GUILayout.Width(31));
            EditorGUILayout.LabelField(new GUIContent("Luck:", "TODO: tooltip"), GUILayout.Width(50));
            EditorGUILayout.LabelField(new GUIContent(data.Luck.ToString(), "TODO: tooltip"), Bold, GUILayout.Width(31));
            EditorGUILayout.EndHorizontal();
        }
        public static void CasterStatsModifierGUILayout(CasterStats stats)
        {
            string data = string.Empty;
            if (stats.Atk != 0)
                data += ("Atk " + (stats.Atk < 0 ? "" : "+") + stats.Atk + ", ");
            if (stats.Def != 0)
                data += ("Def " + (stats.Def < 0 ? "" : "+") + stats.Def + ", ");
            if (stats.Spd != 0)
                data += ("Spd " + (stats.Spd < 0 ? "" : "+") + stats.Spd + ", ");
            if (stats.Acc != 0)
                data += ("Acc " + (stats.Acc < 0 ? "" : "+") + stats.Acc + ", ");
            if (stats.Evade != 0)
                data += ("Evade " + (stats.Evade < 0 ? "" : "+") + stats.Evade);
            if (!string.IsNullOrEmpty(data))
                EditorGUILayout.PrefixLabel(data, new GUIStyle(GUI.skin.label) { wordWrap = true });
        }
        public static void CasterTagDictionaryGUILayout(CasterTagDictionary dict)
        {
            #region Adding
            Event e = Event.current;
            if (e.type == EventType.ExecuteCommand && e.commandName == "ObjectSelectorClosed")
            {
                CasterTag item = EditorGUIUtility.GetObjectPickerObject() as CasterTag;
                if (item == null)
                    return;
                dict.Add(item);
                e.Use();
                return;
            }
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Tags: " + dict.TopLevelTags.Count(), Bold, GUILayout.MaxWidth(120));
            GUILayout.Space(-20);
            if (GUILayout.Button("+"))
                EditorGUIUtility.ShowObjectPicker<CasterTag>(null, false, "", 1);
            GUILayout.EndHorizontal();
            Separator();
            #endregion

            EditorGUI.indentLevel++;
            CasterTag toDelete = default;
            bool delete = false;
            CasterTag[] items = dict.TopLevelTags.ToArray();
            System.Array.Sort(items);
            foreach (var item in items)
            {
                GUILayout.BeginHorizontal();
                CasterTagGUI(item);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("-", GUILayout.Width(45)))
                {
                    toDelete = item;
                    delete = true;
                }
                GUILayout.EndHorizontal();
            }
            if (delete)
                dict.Remove(toDelete);
            EditorGUI.indentLevel--;
            Separator();
        }
        public static void CasterTagGUI(CasterTag data)
        {
            GUILayout.BeginVertical();
            EditorGUILayout.LabelField(data.name + " (" + data.internalName + ")", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold }, GUILayout.Width(200));
            EditorGUI.indentLevel++;
            if (data.subTags.Count > 0)
            {
                EditorGUILayout.PrefixLabel("Subtags: ", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
                foreach (var tag in data.subTags)
                    DoSubtagNames(tag);
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
        private static void DoSubtagNames(CasterTag data)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField(data.name, new GUIStyle(GUI.skin.label), GUILayout.Width(240));
            foreach (var tag in data.subTags)
                DoSubtagNames(tag);
            EditorGUI.indentLevel--;
        }
    }
}
