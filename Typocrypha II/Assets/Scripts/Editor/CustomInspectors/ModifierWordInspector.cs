using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GUIUtils;
using SerializableCollections.GUIUtils;

[CustomEditor(typeof(ModifierWord))]
[CanEditMultipleObjects]
public class ModifierWordInspector : SpellWordInspector
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var word = target as ModifierWord;
        EditorGUILayout.LabelField("Vfx/Sfx", EditorUtils.BoldCentered);
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("fx"));
        EditorGUILayout.EndVertical();
        EditorUtils.Separator();
        EditorGUILayout.LabelField("Game Properties", EditorUtils.BoldCentered);
        word.direction = EditorUtils.EnumPopup(new GUIContent("Direction"), word.direction);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("specialEffect"));
        word.tagsToAdd.DoGUILayout((tag) => EditorUtils.ObjectField(tag, false), () => word.tagsToAdd.ObjPickerAddGUI(), "Tags to Add");
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
