using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Inspector for TIPS entry meta data.
/// </summary>
[CustomEditor(typeof(TIPSEntryData))]
public class TIPSEntryDataInspector : Editor
{
    TIPSEntryData data;

    public override void OnInspectorGUI()
    {
        data = target as TIPSEntryData;

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        if (data.searchTerms == null) data.searchTerms = new NameSet();
        NameSetGUI("Search Terms", data.searchTerms);

        if (GUI.changed) EditorUtility.SetDirty(data);
    }

    void NameSetGUI(string title, NameSet nameSet)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(title + ": " + nameSet.Count, GUILayout.Width(100));
        nameSet.addField = EditorGUILayout.TextField(nameSet.addField, GUILayout.Width(100));
        if (GUILayout.Button("+") && !string.IsNullOrEmpty(nameSet.addField))
        {
            nameSet.Add(nameSet.addField);
        }
        GUILayout.EndHorizontal();
        EditorGUI.indentLevel++;
        string toDelete = null; // Item to delete; -1 if none chosen
        foreach (string s in nameSet)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(s);
            if (GUILayout.Button("-"))
            {
                toDelete = s;
            }
            GUILayout.EndHorizontal();
        }
        if (toDelete != null)
        {
            nameSet.Remove(toDelete);
        }
        EditorGUI.indentLevel--;
    }
}
