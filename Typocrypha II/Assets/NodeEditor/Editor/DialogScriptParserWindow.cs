using UnityEngine;
using UnityEditor;
using NodeEditorFramework;

public class DialogScriptParserWindow : EditorWindow
{
    public readonly DialogScriptParser parser = new DialogScriptParser();

    [MenuItem("Window/DialogScript/Parser")]
    public static void ShowWindow()
    {
        GetWindow(typeof(DialogScriptParserWindow), false, "DialogScriptParser");
    }

    void OnGUI()
    {
        // Main asset fields
        GUILayout.BeginVertical("Box");

        GUILayout.BeginHorizontal();
        GUILayout.Label("Text Script");
        parser.textScript = EditorGUILayout.ObjectField(parser.textScript, typeof(TextAsset), false) as TextAsset;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Node Canvas");
        parser.canvas = EditorGUILayout.ObjectField(parser.canvas, typeof(NodeCanvas), false) as NodeCanvas;
        GUILayout.EndHorizontal();

        parser.endAndTransition = EditorGUILayout.ToggleLeft("Use End and Transition as Default End Node", parser.endAndTransition);

        GUILayout.EndVertical();
        // Generate button
        if (parser.textScript != null && GUILayout.Button("Generate Canvas", GUILayout.Width(120f)))
        {
            parser.GenerateCanvas();
        }
    }
}
