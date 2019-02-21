using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static partial class EditorUtils
{
    public static void CasterStatsGUILayout(CasterStats data, bool isMod = false)
    {
        if (!isMod)
        {
            data.maxHP = EditorGUILayout.IntField(new GUIContent("Max HP", "TODO: tooltip"), data.maxHP);
            data.maxStagger = EditorGUILayout.IntField(new GUIContent("Max Stagger", "TODO: tooltip"), data.maxStagger);
            data.StaggerTime = EditorGUILayout.FloatField(new GUIContent("Stagger Time", "TODO: tooltip"), data.StaggerTime);
        }
        data.Atk = EditorGUILayout.IntSlider(new GUIContent("Atk", "TODO: tooltip"), data.Atk, CasterStats.statRange.min, CasterStats.statRange.max);
        data.Def = EditorGUILayout.IntSlider(new GUIContent("Def", "TODO: tooltip"), data.Def, CasterStats.statRange.min, CasterStats.statRange.max);
        data.Spd = EditorGUILayout.IntSlider(new GUIContent("Speed", "TODO: tooltip"), data.Spd, CasterStats.statRange.min, CasterStats.statRange.max);
        data.Acc = EditorGUILayout.IntSlider(new GUIContent("Acc", "TODO: tooltip"), data.Acc, CasterStats.statRange.min, CasterStats.statRange.max);
        data.Evade = EditorGUILayout.IntSlider(new GUIContent("Evade", "TODO: tooltip"), data.Evade, CasterStats.statRange.min, CasterStats.statRange.max);
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
}
