using UnityEngine;
using UnityEditor;
using NodeEditorFramework;

public class DialogScriptParserWindow : EditorWindow
{
    public readonly DialogScriptParser parser = new DialogScriptParser();

    public TextAsset textScript;
    public NodeCanvas canvas;
    public bool endAndTransition = true;

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
        textScript = EditorGUILayout.ObjectField(textScript, typeof(TextAsset), false) as TextAsset;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Node Canvas");
        canvas = EditorGUILayout.ObjectField(canvas, typeof(NodeCanvas), false) as NodeCanvas;
        GUILayout.EndHorizontal();

        endAndTransition = EditorGUILayout.ToggleLeft("Use End and Transition as Default End Node", endAndTransition);

        GUILayout.EndVertical();
        // Generate button
        if (textScript != null && GUILayout.Button("Generate Canvas", GUILayout.Width(120f)))
        {
            canvas = parser.GenerateCanvas(textScript, endAndTransition);
        }
    }
}
