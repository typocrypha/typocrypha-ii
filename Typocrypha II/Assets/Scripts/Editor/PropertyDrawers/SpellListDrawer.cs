using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GUIUtils;

[CustomPropertyDrawer(typeof(SpellList))]
public class SpellListDrawer : PropertyDrawer
{
    protected RListGUIProperty rList = null;
    protected float GetElementHeight(SerializedProperty elem, int index)
    {
        var items = elem.FindPropertyRelative("items");
        if (items.arraySize == 0)
            return EditorGUIUtility.singleLineHeight * 2.1f; 
        return EditorGUIUtility.singleLineHeight * (items.arraySize + 1.1f);
    }
    protected void InitList(SerializedProperty property, GUIContent label)
    {
        rList = new RListGUIProperty(property, label, GetElementHeight);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (rList == null)
            InitList(property.FindPropertyRelative("items"), label);
        return rList.Height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (rList == null)
            InitList(property.FindPropertyRelative("items"), label);

        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        rList.DoList(position);

        EditorGUI.EndProperty();
    }
}
