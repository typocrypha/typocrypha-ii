using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class InspectorEditorWindow : EditorWindow
{
    public Object target;
    public static InspectorEditorWindow Create(Object target, string title)
    {
        var window = (InspectorEditorWindow)EditorWindow.GetWindow(typeof(InspectorEditorWindow));
        window.target = target;
        window.titleContent = new GUIContent(title);
        return window;
    }
    private void OnGUI()
    {
        var editor = Editor.CreateEditor(target);
        editor.OnInspectorGUI();
    }
}
