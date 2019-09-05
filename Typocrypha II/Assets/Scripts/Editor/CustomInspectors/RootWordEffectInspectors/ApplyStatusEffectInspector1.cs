using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SerializableCollections.GUIUtils;
using GUIUtils;

[CustomEditor(typeof(ApplyStatusEffect), true)]
public class ApplyStatusEffectInspector : RootWordEffectInspector
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorUtils.Separator();
        var effect = target as ApplyStatusEffect;
        effect.statusEffectPrefab = EditorUtils.ObjectField(new GUIContent("Status Effect Prefab"), effect.statusEffectPrefab, false);
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
