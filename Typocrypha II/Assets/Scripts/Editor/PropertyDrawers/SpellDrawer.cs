using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GUIUtils;

//[CustomPropertyDrawer(typeof(Spell))]
public class SpellDrawer : PropertyDrawer
{
    protected RListGUIProperty rList = null;
    protected void InitList(SerializedProperty property, GUIContent label)
    {
        rList = new RListGUIProperty(property, label);
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
