using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShowDynamicInputPromptEffect), true)]
public class ShowDynamicInputPromptEffectInspector : RootWordEffectInspector
{
    public override bool IsChild => true;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorUtils.Separator();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("index"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("onFail"));
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
