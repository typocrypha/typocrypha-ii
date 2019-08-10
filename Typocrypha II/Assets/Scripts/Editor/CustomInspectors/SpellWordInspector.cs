using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpellWord))]
public class SpellWordInspector : Editor
{
    public override void OnInspectorGUI()
    {
        var word = target as SpellWord;
        EditorGUILayout.LabelField(word.GetType().ToString() + " " + word.name);
        word.internalName = EditorGUILayout.TextField(new GUIContent("Name"), word.internalName);
        if (string.IsNullOrWhiteSpace(word.internalName) || GUILayout.Button("Set name to asset name"))
        {
            if (word.internalName != word.name)
            {
                word.internalName = word.name;
                EditorUtility.SetDirty(word);
            }
        }
        EditorUtils.Separator();
        EditorGUILayout.LabelField(new GUIContent("Description"), EditorUtils.BoldCentered);
        word.description = EditorGUILayout.TextArea(word.description, new GUIStyle(GUI.skin.textArea) { wordWrap = true }, GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 2));
        EditorUtils.Separator();
    }
}
