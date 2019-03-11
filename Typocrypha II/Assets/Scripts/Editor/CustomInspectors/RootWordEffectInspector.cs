using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GUIUtils;

[CustomEditor(typeof(RootWordEffect), true)]
public class RootWordEffectInspector : Editor
{
    public override void OnInspectorGUI()
    {
        var effect = target as RootWordEffect;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("fx"));
        EditorUtils.Separator();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("pattern"));
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
            EditorUtility.SetDirty(this);
    }
}
