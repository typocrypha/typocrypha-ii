using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityGoogleDrive;

public class DialogScriptUpdater : EditorWindow
{
    string query;
    string exportLocation;

    bool toggleSelectAll;
    bool toggleParse = true;

    const string PLAYERPREF_QUERY = "GOOGLE_API_LIST_QUERY";
    const string PLAYERPREF_EXPORT = "GOOGLE_API_EXPORT_PATH";

    List<UnityGoogleDrive.Data.File> fileResults;
    List<bool> fileSelection;

    Vector2 scrollPosition;

    private void OnEnable()
    {
        query = PlayerPrefs.HasKey(PLAYERPREF_QUERY) ? PlayerPrefs.GetString(PLAYERPREF_QUERY) : default;
        exportLocation = PlayerPrefs.HasKey(PLAYERPREF_EXPORT) ? PlayerPrefs.GetString(PLAYERPREF_EXPORT) : default;
    }

    private void OnDisable()
    {
        PlayerPrefs.SetString(PLAYERPREF_QUERY, query);
        PlayerPrefs.SetString(PLAYERPREF_EXPORT, exportLocation);
    }

    [MenuItem("Window/DialogScript/Updater")]
    public static void ShowWindow()
    {
        GetWindow(typeof(DialogScriptUpdater));
    }

    private void OnGUI()
    {
        //styles
        GUIStyle LargeButton = new GUIStyle("LargeButton") { stretchWidth = false };
        GUIStyle Button = new GUIStyle("Button") { stretchWidth = false };
        GUIStyle RightAligned = new GUIStyle() { alignment = TextAnchor.MiddleRight, padding = new RectOffset(0,10,0,0)};
        GUIStyle TextArea = new GUIStyle("TextArea") { wordWrap = false };

        //premature exit conditions
        GoogleDriveSettings settings = GoogleDriveSettings.LoadFromResources(true);
        if (!settings)
        {
            GUILayout.Label("Please create a new GoogleDriveSettings asset.\n(Edit > Project Settings > Google Drive Settings)");
            return;
        }
        if (!settings.GenericClientCredentials.ContainsSensitiveData())
        {
            GUILayout.Label("Please configure your credentials.\n(Edit > Project Settings > Google Drive Settings)");
            return;
        }
        if (!settings.IsAnyAuthTokenCached())
        {
            GUILayout.Label("Please sign into Google account.\n(Tools > UnityGoogleDrive > Log In)");
            return;
        }

        //query
        GUILayout.Label("Query", EditorStyles.boldLabel);
        query = GUILayout.TextArea(query, TextArea, GUILayout.MinHeight(45));

        if (GUILayout.Button("Search", LargeButton))
        {
            var request = ListFiles(query);
            request.Send().OnDone += (response) =>
            {
                if (request.IsError) return;
                fileResults = response.Files;
                fileSelection = Enumerable.Repeat(false, fileResults.Count).ToList();
            };
        }

        if (fileResults == null) return;
        EditorGUILayout.Space();

        //results
        GUILayout.Label("Search Results", EditorStyles.boldLabel);
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        GUILayout.BeginHorizontal();
        bool selectAllDirty = EditorGUILayout.ToggleLeft("Select All", toggleSelectAll) != toggleSelectAll;
        if (selectAllDirty)
        {
            fileSelection = Enumerable.Repeat(!toggleSelectAll, fileResults.Count).ToList();
            toggleSelectAll = !toggleSelectAll;
        }
        GUILayout.Label("Modified", RightAligned);
        GUILayout.EndHorizontal();

        for (int i = 0; i < fileResults.Count; ++i)
        {
            GUILayout.BeginHorizontal();
            fileSelection[i] = EditorGUILayout.ToggleLeft(fileResults[i].Name, fileSelection[i]);
            if (fileResults[i].ModifiedTime is DateTime dt)
                GUILayout.Label(dt.ToString("MM/dd/yy"), RightAligned);
            GUILayout.EndHorizontal();
        }
        bool enableExport = fileSelection.Contains(true);
        GUILayout.EndScrollView();

        EditorGUILayout.Space();

        //export options
        GUILayout.Label("Export", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        GUILayout.Label($"Path: Assets{Path.DirectorySeparatorChar}", GUILayout.Width(82));
        exportLocation = EditorGUILayout.DelayedTextField(exportLocation);
        GUILayout.EndHorizontal();
        toggleParse = EditorGUILayout.ToggleLeft("Parse After Export", toggleParse);

        //export
        GUI.enabled = enableExport;
        if (GUILayout.Button("Export Selected", LargeButton))
        {
            for (int i = 0; i < fileResults.Count; ++i)
            {
                if (!fileSelection[i]) continue;

                try
                {
                    ExportDocument(
                        fileResults[i].Id,
                        $"{ fileResults[i].Name}.txt",
                        Path.Combine(Application.dataPath, exportLocation),
                        toggleParse ? ParseOnExport : null as Action<string>);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
        GUI.enabled = true;
    }


    [MenuItem("Tools/UnityGoogleDrive/Log In")]
    static void LogIn()
    {
        AuthController.CancelAuth();
        AuthController.RefreshAccessToken();
    }

    [MenuItem("Tools/UnityGoogleDrive/Log Out")]
    static void LogOut()
    {
        GoogleDriveSettings.LoadFromResources().DeleteCachedAuthTokens();
    }

    static GoogleDriveFiles.ListRequest ListFiles(string query)
    {
        return GoogleDriveFiles.List(query, new List<string> { "files(id,name,modifiedTime)" });
    }

    static void ExportDocument(string id, string fileName, string exportPath, Action<string> OnDone = null)
    {
        var request = GoogleDriveFiles.Export(id, "text/plain");
        request.Send().OnDone += file =>
        {
            if (request.IsError)
            {
                Debug.LogError(request.Error);
                return;
            }
            string path = Path.Combine(exportPath, fileName);
            using (var writer = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                writer.Write(file.Content);
                writer.Flush();
                writer.Close();
            }
            Debug.Log($"Exported to {path}");
            AssetDatabase.Refresh();
            OnDone.Invoke(path);
        };
    }

    static void ParseOnExport(string filePath)
    {
        if (filePath.StartsWith(Application.dataPath))
            filePath = $"Assets{filePath.Substring(Application.dataPath.Length)}";

        new DialogScriptParser().GenerateCanvas(AssetDatabase.LoadAssetAtPath<TextAsset>(filePath), true);
    }
}