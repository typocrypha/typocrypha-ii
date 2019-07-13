using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SubSOAttribute))]
public class SubSOAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, GUIContent.none, property);
        var attr = attribute as SubSOAttribute;
        var UIRect = new Rect(position) { height = EditorGUIUtility.singleLineHeight };
        var dropRect = new Rect(UIRect) { width = UIRect.width - 125 };
        string dropString = "Set " + attr.name + ": " + (property.objectReferenceValue == null ? "(None)" : property.objectReferenceValue.ToString());
        if (EditorGUI.DropdownButton(dropRect, new GUIContent(dropString, dropString), FocusType.Keyboard))
        {
            GetMenu(property, fieldInfo.FieldType).ShowAsContext();
        }
        var inspectButtonRect = new Rect(UIRect) { x = UIRect.x + dropRect.width + 3, width = 122};
        if (property.objectReferenceValue != null && GUI.Button(inspectButtonRect, "View Inspector"))
        {
            var inspector = InspectorEditorWindow.Create(property.objectReferenceValue, attr.name);
            inspector.ShowAuxWindow();
        }      
        property.serializedObject.ApplyModifiedProperties();
        EditorGUI.EndProperty();
    }
    private GenericMenu GetMenu(SerializedProperty property, System.Type objType)
    {
        var menu = new GenericMenu();
        // Add an option to reset to none
        menu.AddItem(new GUIContent("None"), false, (obj) =>
        {
            if (property.objectReferenceValue != null)
                Object.DestroyImmediate(property.objectReferenceValue, true);
            property.objectReferenceValue = null;
            property.serializedObject.ApplyModifiedProperties();
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(property.serializedObject.targetObject));
        }, null);
        // Add an option for each subtype of the given object type
        foreach (var type in ReflectionUtils.ReflectiveEnumerator.GetAllSubclassTypes(objType))
        {
            string[] path = type.ToString().Split('.');
            var name = path[path.Length - 1];
            menu.AddItem(new GUIContent(name), false, (obj) =>
            {
                var newItem = ScriptableObject.CreateInstance((System.Type)obj);
                newItem.hideFlags = HideFlags.HideInHierarchy;
                if(property.objectReferenceValue != null)
                    Object.DestroyImmediate(property.objectReferenceValue, true);
                AssetDatabase.AddObjectToAsset(newItem, property.serializedObject.targetObject);
                property.objectReferenceValue = newItem;              
                property.serializedObject.ApplyModifiedProperties();
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(property.serializedObject.targetObject));
            }
            , type);
        }
        return menu;
    }
}
