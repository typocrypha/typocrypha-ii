using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpawnBattleWordEffectDynamic), true)]
public class SpawnBattleWordEffectDynamicInspector : RootWordEffectInspector
{
    public override bool IsChild => true;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorUtils.Separator();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("key"));
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
