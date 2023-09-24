﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ApplyStatusEffect), true)]
public class ApplyStatusEffectInspector : RootWordEffectInspector
{
    public override bool IsChild => true;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorUtils.Separator();
        var effect = target as ApplyStatusEffect;
        effect.statusEffectPrefab = EditorUtils.ObjectField(new GUIContent("Status Effect Prefab"), effect.statusEffectPrefab, false);
        effect.canCrit = EditorGUILayout.ToggleLeft(new GUIContent("Can Crit"), effect.canCrit);
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
