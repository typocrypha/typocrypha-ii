using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CallRandomReinforcementsEffect), true)]
public class CallRandomReinforcementsEffectInspector : RootWordEffectInspector
{
    public override bool IsChild => true;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorUtils.Separator();
        var effect = target as CallRandomReinforcementsEffect;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("reinforcementPrefabs"), true);
        effect.number = EditorGUILayout.IntSlider(new GUIContent("Number"), effect.number, 1, 2);
        effect.preventRepeats = EditorGUILayout.ToggleLeft(new GUIContent("Prevent Repeats"), effect.preventRepeats);
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
