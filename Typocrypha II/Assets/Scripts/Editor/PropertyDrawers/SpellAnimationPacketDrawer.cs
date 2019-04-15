using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(AnimationPacket))]
public class SpellAnimationPacketDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, GUIContent.none, property);
        const float labelWidth = 32;
        float fieldWidth = (position.width - (labelWidth * 2) - 4) / 2;

        Rect UIRect = new Rect(position) { height = EditorGUIUtility.singleLineHeight, width = labelWidth };
        EditorGUI.PrefixLabel(UIRect, new GUIContent("Anim"));
        UIRect.x += labelWidth + 1;
        UIRect.width = fieldWidth;
        EditorGUI.PropertyField(UIRect, property.FindPropertyRelative("clip"), GUIContent.none);
        UIRect.x += fieldWidth + 1;
        UIRect.width = labelWidth;
        EditorGUI.PrefixLabel(UIRect, new GUIContent("Sfx"));
        UIRect.x += labelWidth + 1;
        UIRect.width = fieldWidth;
        EditorGUI.PropertyField(UIRect, property.FindPropertyRelative("sfx"), GUIContent.none);

        EditorGUI.EndProperty();
    }
}
