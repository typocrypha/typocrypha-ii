using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SerializableCollections.GUIUtils;
using GUIUtils;

[CustomEditor(typeof(ApplyRuleEffect), true)]
public class ApplyRuleEffectInspector : RootWordEffectInspector
{
    public override bool IsChild => true;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorUtils.Separator();
        var effect = target as ApplyRuleEffect;
        effect.ruleType = EditorUtils.EnumPopup(new GUIContent("Rule"), effect.ruleType);
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
