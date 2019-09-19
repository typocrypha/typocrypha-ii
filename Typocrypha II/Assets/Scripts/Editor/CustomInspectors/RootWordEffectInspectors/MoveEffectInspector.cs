using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SerializableCollections.GUIUtils;
using GUIUtils;

[CustomEditor(typeof(MoveEffect), true)]
public class MoveEffectInspector : RootWordEffectInspector
{
    public override bool IsChild => true;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorUtils.Separator();
        var effect = target as MoveEffect;
        effect.onlyMoveToUnoccupied = EditorGUILayout.ToggleLeft(new GUIContent("Only move to unoccupied"), effect.onlyMoveToUnoccupied);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("positions"), true);
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
