using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpawnBattleWordEffectFixed), true)]
public class SpawnBattleWordEffectFixedInspector : RootWordEffectInspector
{
    public override bool IsChild => true;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorUtils.Separator();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("sequencePosition"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("data"), true);
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
