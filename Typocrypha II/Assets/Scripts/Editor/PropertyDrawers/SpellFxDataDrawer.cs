using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GUIUtils;

[CustomPropertyDrawer(typeof(SpellFxData))]
public class SpellFxDataDrawer : PropertyDrawer
{
    protected RListGUIProperty GetRList(SerializedProperty property)
    {
        return new RListGUIProperty(property.FindPropertyRelative("effectPackets"), new GUIContent("Sfx/Vfx"));
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        switch (property.FindPropertyRelative("effectType").enumValueIndex)
        {
            case (int)SpellFxData.EffectType.None:
                return EditorGUIUtility.singleLineHeight + 1;
            case (int)SpellFxData.EffectType.Single:
                return EditorGUIUtility.singleLineHeight * 2 + 1;               
            case (int)SpellFxData.EffectType.Sequence:
                return GetRList(property).Height;
            case (int)SpellFxData.EffectType.Parallel:
                return GetRList(property).Height;
            case (int)SpellFxData.EffectType.Prefab:
                return EditorGUIUtility.singleLineHeight * 2 + 1;
        }
        return EditorGUIUtility.singleLineHeight * 2;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        Rect UIRect = new Rect(position) { height = EditorGUIUtility.singleLineHeight };
        //EditorGUILayout.LabelField(new GUIContent("Animation and Sfx"));

        EditorGUI.PropertyField(UIRect, property.FindPropertyRelative("effectType"), new GUIContent("FxType"));
        UIRect.y += EditorGUIUtility.singleLineHeight + 1;
        switch (property.FindPropertyRelative("effectType").enumValueIndex)
        {
            case (int)SpellFxData.EffectType.Single:
                EditorGUI.PropertyField(UIRect, property.FindPropertyRelative("effectPackets").GetArrayElementAtIndex(0));
                break;
            case (int)SpellFxData.EffectType.Sequence:
                GetRList(property).DoList(UIRect);
                break;
            case (int)SpellFxData.EffectType.Parallel:
                GetRList(property).DoList(UIRect);
                break;
            case (int)SpellFxData.EffectType.Prefab:
                EditorGUI.PropertyField(UIRect, property.FindPropertyRelative("effectPrefab"));
                break;
        }
        if (GUI.changed)
        {
            property.serializedObject.ApplyModifiedProperties();
        }
        EditorGUI.EndProperty();
    }
}
