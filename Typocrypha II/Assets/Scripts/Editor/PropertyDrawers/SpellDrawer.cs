using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GUIUtils;

[CustomPropertyDrawer(typeof(Spell))]
public class SpellDrawer : PropertyDrawer
{
    private const float controlBlockWidth = 100;
    private const float controlWidth = controlBlockWidth * 0.25f;
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var items = property.FindPropertyRelative("items");
        if (items.arraySize == 0)
            return EditorGUIUtility.singleLineHeight * 2.1f;
        return EditorGUIUtility.singleLineHeight * (items.arraySize + 1.1f);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Rect UIRect = new Rect(position) { height = EditorGUIUtility.singleLineHeight, width = position.width - controlBlockWidth };
        Rect ControlRect = new Rect(UIRect) { x = position.x + position.width - controlBlockWidth, width = controlWidth };
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);
        if(label.text == string.Empty)
            GUI.Label(new Rect(position) { height = EditorGUIUtility.singleLineHeight }, "Spell", EditorUtils.Bold);
        else
            GUI.Label(new Rect(position) { height = EditorGUIUtility.singleLineHeight }, label, EditorUtils.Bold);
        UIRect.y += EditorGUIUtility.singleLineHeight;
        ControlRect.y += EditorGUIUtility.singleLineHeight;

        var items = property.FindPropertyRelative("items");
        for(int i = 0; i < items.arraySize; ++i)
        {
            EditorGUI.PropertyField(UIRect, items.GetArrayElementAtIndex(i), GUIContent.none);
            if (i != 0 && GUI.Button(ControlRect, new GUIContent("∧", "move this element up")))
                items.MoveArrayElement(i, i - 1);
            ControlRect.x += controlWidth;
            if (i != items.arraySize - 1 && GUI.Button(ControlRect, new GUIContent("∨", "move this element down")))
                items.MoveArrayElement(i, i + 1);
            ControlRect.x += controlWidth;
            if (GUI.Button(ControlRect, new GUIContent("-", "delete this element")))
            {
                int oldSize = items.arraySize;
                items.DeleteArrayElementAtIndex(i);
                if (items.arraySize == oldSize)
                    items.DeleteArrayElementAtIndex(i);
            }
            ControlRect.x += controlWidth;
            if (GUI.Button(ControlRect, new GUIContent("+", "insert a new element after this one")))
                items.InsertArrayElementAtIndex(i);       
            UIRect.y += EditorGUIUtility.singleLineHeight;
            ControlRect.y += EditorGUIUtility.singleLineHeight;
            ControlRect.x = position.x + position.width - controlBlockWidth;
        }
        if(items.arraySize == 0)
        {
            if(GUI.Button(new Rect(UIRect) { width = position.width }, new GUIContent("+ Word")))
            {
                items.InsertArrayElementAtIndex(items.arraySize);
            }
        }

        EditorGUI.EndProperty();
    }
}
