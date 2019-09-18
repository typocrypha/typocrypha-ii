using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SerializableCollections.GUIUtils;
using GUIUtils;

[CustomEditor(typeof(AddTagsEffect), true)]
public class AddTagsEffectInspector : RootWordEffectInspector
{
    public override bool IsChild => true;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorUtils.Separator();
        var effect = target as AddTagsEffect;
        void kGUI(CasterTag t) => EditorGUILayout.LabelField(t.internalName);
        effect.casterTagsToAdd.DoGUILayout(kGUI,() => effect.casterTagsToAdd.ObjPickerAddGUI(), "C Tags To Add");
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
