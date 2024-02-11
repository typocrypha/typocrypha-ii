using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SerializableCollections.GUIUtils;
using GUIUtils;

[CustomEditor(typeof(RemoveTagsEffect), true)]
public class RemoveTagsEffectInspector : RootWordEffectInspector
{
    public override bool IsChild => true;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorUtils.Separator();
        var effect = target as RemoveTagsEffect;
        void kGUI(CasterTag t) => EditorGUILayout.LabelField(t.internalName);
        effect.casterTagsToRemove.DoGUILayout(kGUI,() => effect.casterTagsToRemove.ObjPickerAddGUI(), "Tags To Remove");
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
