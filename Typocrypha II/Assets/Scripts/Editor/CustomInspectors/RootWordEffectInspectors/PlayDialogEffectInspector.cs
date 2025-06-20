using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayDialogEffect), true)]
public class PlayDialogEffectInspector : RootWordEffectInspector
{
    public override bool IsChild => true;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorUtils.Separator();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("dialog"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("auto"));
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
