using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ApplyKeyEffectRandom), true)]
public class ApplyKeyEffectSpecificInspector : RootWordEffectInspector
{
    public override bool IsChild => true;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorUtils.Separator();
        var effect = target as ApplyKeyEffectRandom;
        effect.keyEffectPrefab = EditorUtils.ObjectField(new GUIContent("Key Effect Prefab"), effect.keyEffectPrefab, false);
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
