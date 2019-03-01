//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//[CustomPropertyDrawer(typeof(TestRootEffect))]
//public class TestRootEffectDrawer : PropertyDrawer
//{
//    private static readonly float lineHeight = EditorGUIUtility.singleLineHeight + 1;
//    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//    {
//        return (lineHeight) * 7;
//    }
//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
//        // Using BeginProperty / EndProperty on the parent property means that
//        // prefab override logic works on the entire property.
//        EditorGUI.BeginProperty(position, label, property);

//        //Initialize properties

//        //Initialize UI variables
        
//        Rect UIRect = new Rect(position) { height = EditorGUIUtility.singleLineHeight };
//        EditorGUI.PropertyField(UIRect, property.FindPropertyRelative("debugMessage"));
//        UIRect.y += lineHeight;
//        EditorGUI.PropertyField(UIRect, property.FindPropertyRelative("effect"));
//        UIRect.y += lineHeight;
//        UIRect.height = lineHeight * 5;
//        EditorGUI.PropertyField(UIRect, property.FindPropertyRelative("pattern"));


//        // Set indent back to what it was
//        //EditorGUI.indentLevel = indent;

//        EditorGUI.EndProperty();
//    }
//}
