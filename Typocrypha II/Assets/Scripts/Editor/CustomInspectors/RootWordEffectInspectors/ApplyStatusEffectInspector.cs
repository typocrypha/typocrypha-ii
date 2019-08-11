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
        effect.effectPrefab = EditorUtils.ObjectField(new GUIContent("Effect Prefab"), effect.effectPrefab, false);
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
