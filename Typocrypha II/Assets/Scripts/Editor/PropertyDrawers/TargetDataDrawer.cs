using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(TargetData))]
public class TargetDataDrawer : PropertyDrawer
{
    const float checkWidth = 50;
    const float labelWidth = 100;
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return (EditorGUIUtility.singleLineHeight + 1) * 5;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        //Initialize properties
        var arrayProp = property.FindPropertyRelative("pattern").FindPropertyRelative("_data");
        var typeProp = property.FindPropertyRelative("type");
        //Initialize UI variables
        float lineHeight = EditorGUIUtility.singleLineHeight + 1;
        Rect UIRect = new Rect(position) { height = EditorGUIUtility.singleLineHeight };

        #region Actual GUI drawing
        GUI.Label(UIRect, new GUIContent("Target Pattern", "TODO, tooltip"));
        UIRect.y += lineHeight;
        EditorGUI.PrefixLabel(new Rect(UIRect) { width = labelWidth }, new GUIContent("Type"));
        EditorGUI.PropertyField(new Rect(UIRect) { x = UIRect.x + labelWidth, width = UIRect.width - labelWidth }, typeProp, GUIContent.none);
        UIRect.y += lineHeight;

        #region Draw Matrix

        //Draw Top row
        string topRowLabel = (TargetData.Type)typeProp.enumValueIndex == TargetData.Type.Targeted ? "Target Row" : "Enemy Row";
        EditorGUI.PrefixLabel(new Rect(UIRect) { width = labelWidth }, new GUIContent(topRowLabel));
        Rect checkRect = new Rect(UIRect) { width = checkWidth, x = UIRect.x + labelWidth + 3 };
        EditorGUI.PropertyField(checkRect, arrayProp.GetArrayElementAtIndex(0), GUIContent.none);
        checkRect.x += checkWidth;
        EditorGUI.PropertyField(checkRect, arrayProp.GetArrayElementAtIndex(1), GUIContent.none);
        checkRect.x += checkWidth;
        EditorGUI.PropertyField(checkRect, arrayProp.GetArrayElementAtIndex(2), GUIContent.none);
        UIRect.y += lineHeight;

        //Draw Bottom row
        string bottomRowLabel = (TargetData.Type)typeProp.enumValueIndex == TargetData.Type.Targeted ? "Other Row" : "Caster Row";
        EditorGUI.PrefixLabel(new Rect(UIRect) { width = labelWidth }, new GUIContent(bottomRowLabel));
        checkRect = new Rect(UIRect) { width = checkWidth, x = UIRect.x + labelWidth + 3 };
        EditorGUI.PropertyField(checkRect, arrayProp.GetArrayElementAtIndex(3), GUIContent.none);
        checkRect.x += checkWidth;
        EditorGUI.PropertyField(checkRect, arrayProp.GetArrayElementAtIndex(4), GUIContent.none);
        checkRect.x += checkWidth;
        EditorGUI.PropertyField(checkRect, arrayProp.GetArrayElementAtIndex(5), GUIContent.none);
        UIRect.y += lineHeight;
        #endregion

        #endregion

        // Set indent back to what it was
        //EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
