using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ApplyKeyEffectSpecific), true)]
public class ApplyKeyEffectRandomInspector : RootWordEffectInspector
{
    public override bool IsChild => true;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorUtils.Separator();
        var effect = target as ApplyKeyEffectSpecific;
        effect.keyEffectPrefab = EditorUtils.ObjectField(new GUIContent("Key Effect Prefab"), effect.keyEffectPrefab, false);
        effect.affectedKeys = EditorGUILayout.TextField(new GUIContent("Affected Keys"), effect.affectedKeys);
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
