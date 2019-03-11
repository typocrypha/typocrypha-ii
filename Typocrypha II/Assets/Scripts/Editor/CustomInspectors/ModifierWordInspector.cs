using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GUIUtils;
using SerializableCollections.GUIUtils;

[CustomEditor(typeof(ModifierWord))]
public class ModifierWordInspector : Editor
{
    public override void OnInspectorGUI()
    {
        var word = target as ModifierWord;
        EditorGUILayout.LabelField("Modifer Word: " + word.name);
        word.displayName = EditorGUILayout.TextField(new GUIContent("Display Name"), word.displayName);
        EditorUtils.Separator();
        EditorGUILayout.LabelField(new GUIContent("Description"), EditorUtils.BoldCentered);
        word.description = EditorGUILayout.TextArea(word.description, new GUIStyle(GUI.skin.textArea) { wordWrap = true }, GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 2));
        EditorUtils.Separator();
        EditorGUILayout.LabelField("Vfx/Sfx", EditorUtils.BoldCentered);
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("fx"));
        EditorGUILayout.EndVertical();
        EditorUtils.Separator();
        EditorGUILayout.LabelField("Game Properties", EditorUtils.BoldCentered);
        word.direction = EditorUtils.EnumPopup(new GUIContent("Direction"), word.direction);
        word.specialEffect = EditorUtils.ObjectField(new GUIContent("Special Effect", "TODO: Tooltip"), word.specialEffect, false);
        word.tagsToAdd.DoGUILayout((tag) => EditorUtils.ObjectField(tag, false), () => word.tagsToAdd.ObjPickerAddGUI(), "Tags to Add");
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
