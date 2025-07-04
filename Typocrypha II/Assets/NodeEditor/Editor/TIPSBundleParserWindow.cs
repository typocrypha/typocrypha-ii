using UnityEngine;
using UnityEditor;
using NodeEditorFramework;
using System.IO;

public class TIPSBundleParserWindow : EditorWindow
{
    public TextAsset csvFile;

    [MenuItem("Window/TIPSBundleParser")]
    public static void ShowWindow()
    {
        GetWindow(typeof(TIPSBundleParserWindow), false, "TIPSBundleParser");
    }

    void OnGUI()
    {
        // Main asset fields
        GUILayout.BeginVertical("Box");

        GUILayout.BeginHorizontal();
        GUILayout.Label("TIPS Entries CSV");
        csvFile = EditorGUILayout.ObjectField(csvFile, typeof(TextAsset), false) as TextAsset;
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        string assetPath = AssetDatabase.GetAssetPath(csvFile.GetInstanceID());
        if (assetPath != null && GUILayout.Button("Build TIPS", GUILayout.Width(120f)))
        {
            const string assetPathPrefix = "Assets";
            var fullPath = Application.dataPath + assetPath.Substring(assetPathPrefix.Length);
            TIPSBundleParser.Parse(fullPath);
        }
    }
}
