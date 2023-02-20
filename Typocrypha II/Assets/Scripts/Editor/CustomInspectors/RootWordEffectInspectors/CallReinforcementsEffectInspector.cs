using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SerializableCollections.GUIUtils;
using GUIUtils;

[CustomEditor(typeof(CallReinforcementsEffect), true)]
public class CallReinforcementsEffectInspector : RootWordEffectInspector
{
    public override bool IsChild => true;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorUtils.Separator();
        var effect = target as CallReinforcementsEffect;
        effect.reinforcementPrefab = EditorUtils.ObjectField(new GUIContent("Reinforcment Prefab"), effect.reinforcementPrefab, false);
        effect.number = EditorGUILayout.IntSlider(new GUIContent("Number"), effect.number, 1, 2);
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
