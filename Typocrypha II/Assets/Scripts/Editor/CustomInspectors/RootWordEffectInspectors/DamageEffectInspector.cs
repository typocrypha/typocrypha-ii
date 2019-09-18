using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GUIUtils;

[CustomEditor(typeof(DamageEffect), true)]
public class DamageEffectInspector : RootWordEffectInspector
{
    public override bool IsChild => true;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorUtils.Separator();
        var effect = target as DamageEffect;
        effect.power = EditorGUILayout.IntField(new GUIContent("Power"), effect.power);
        effect.formula = EditorUtils.EnumPopup(new GUIContent("FormulaType"), effect.formula);
        if (effect.formula == Damage.FormulaType.Custom)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("customFormula"));
            serializedObject.ApplyModifiedProperties();
        }
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
