﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// CharacterData inspector (read-only)
[CustomEditor(typeof(CharacterData))]
public class CharacterDataInspector : Editor
{
    CharacterData data;

    public override void OnInspectorGUI()
    {
        data = target as CharacterData;
        GUILayout.Label("Character: " + data.name);

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        if (data.aliases == null)
        {
            data.aliases = new NameSet();
        }
        NameSetGUI("Aliases", data.aliases);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        if (data.poses == null)
        {
            data.poses = new NameMap();
        }
        NameMapGUI("Poses", data.poses);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        if (data.expressions == null)
        {
            data.expressions = new NameMap();
        }
        NameMapGUI("Expressions", data.expressions);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        if (data.bodies == null)
        {
            data.bodies = new NameMap();
        }
        NameMapGUI("Bodies", data.bodies);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        if (data.clothes == null)
        {
            data.clothes = new NameMap();
        }
        NameMapGUI("Clothes", data.clothes);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        if (data.hair == null)
        {
            data.hair = new NameMap();
        }
        NameMapGUI("Hair", data.hair);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        if (data.codecs == null)
        {
            data.codecs = new NameMap();
        }
        NameMapGUI("Codecs", data.codecs);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        data.chat_icon = EditorGUILayout.ObjectField("Chat Icon", data.chat_icon, typeof(Sprite), false) as Sprite;
        data.talk_sfx = EditorGUILayout.ObjectField("Talking SFX", data.talk_sfx, typeof(UnityEngine.AudioClip), false) as UnityEngine.AudioClip;
        if (GUI.changed)
        {
            EditorUtility.SetDirty(data);
        }

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
        GUILayout.BeginHorizontal();
        GUILayout.Label("    Names");
        GUILayout.EndHorizontal();
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

    void NameMapGUI(string title, NameMap nameMap)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(title + ": " + nameMap.Count, GUILayout.Width(100));
        nameMap.addField = EditorGUILayout.TextField(nameMap.addField, GUILayout.Width(100));
        if (GUILayout.Button("+") && !string.IsNullOrEmpty(nameMap.addField))
            nameMap.Add(nameMap.addField, null);
        GUIStyle header = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold };
        GUILayout.EndHorizontal();
        EditorGUI.indentLevel++;
        GUILayout.BeginHorizontal();
        GUILayout.Label("    Names", header);
        GUILayout.Label("Sprites", header);
        GUILayout.EndHorizontal();
        string toDelete = null; // Item to delete; -1 if none chosen
        string[] keys = new string[nameMap.Count];
        nameMap.Keys.CopyTo(keys, 0);
        foreach (var key in keys)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(key, GUILayout.Width(100));
            nameMap[key] = EditorGUILayout.ObjectField(nameMap[key], typeof(Sprite), false) as Sprite;
            if (GUILayout.Button("-"))
                toDelete = key;
            GUILayout.EndHorizontal();
        }
        if (toDelete != null)
            nameMap.Remove(toDelete);
        EditorGUI.indentLevel--;
    }
}
