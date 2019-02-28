using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SerializableCollections.GUIUtils;

[CustomEditor(typeof(CasterTag))]
public class CasterTagInspector : Editor
{
    public override void OnInspectorGUI()
    {
        CasterTag tag = target as CasterTag;
        EditorGUILayout.LabelField("Caster Tag: " + tag.name + " (" + tag.displayName + ")");
        EditorUtils.Separator();
        tag.displayName = EditorGUILayout.TextField(new GUIContent("Display Name"), tag.displayName);
        EditorUtils.Separator();
        EditorGUILayout.LabelField("Stat Modifiers", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter });
        if (tag.statMods != null)
            EditorUtils.CasterStatsGUILayout(tag.statMods, true);
        EditorGUILayout.Space();
        EditorUtils.Separator();
        if (tag.subTags != null)
        {
            tag.subTags.DoGUILayout(EditorUtils.CasterTagGUI, tag.subTags.ObjPickerAddGUI, "SubTags");
            if (tag.subTags.Contains(tag))
                tag.subTags.Remove(tag);
        }
        if (GUI.changed)
            EditorUtility.SetDirty(tag);
    }
}
