using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SerializableCollections.GUIUtils;

[CustomEditor(typeof(RootWordEffect), true)]
public class RootWordEffectInspector : Editor
{
    public override void OnInspectorGUI()
    {
        var effect = target as RootWordEffect;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("fx"));
        EditorUtils.Separator();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("pattern"));
        EditorUtils.Separator();
        effect.tags.DoGUILayout((tag) => EditorGUILayout.LabelField(tag.name), () => effect.tags.ObjPickerAddGUI(), "Tags");
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
