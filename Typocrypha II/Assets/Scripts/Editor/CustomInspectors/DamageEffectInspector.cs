﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GUIUtils;

[CustomEditor(typeof(DamageEffect), true)]
public class DamageEffectInspector : RootWordEffectInspector
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorUtils.Separator();
        var effect = target as DamageEffect;
        effect.power = EditorGUILayout.IntField(new GUIContent("Power"), effect.power);
        effect.formula = EditorUtils.EnumPopup(new GUIContent("FormulaType"), effect.formula);
        if (effect.formula == Damage.FormulaType.Custom)
            effect.customFormula = EditorUtils.ObjectField(effect.customFormula, false);
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
