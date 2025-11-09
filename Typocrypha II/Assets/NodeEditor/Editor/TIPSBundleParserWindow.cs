using UnityEngine;
using UnityEditor;

public class TIPSBundleParserWindow : EditorWindow
{
    public TextAsset csvFile;
    public SpellWordBundle spellBundle;
    public CasterBundle casterBundle;

    private const float BUTTON_WIDTH = 160f;

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

        GUILayout.BeginHorizontal();
        GUILayout.Label("Caster Bundle");
        casterBundle = EditorGUILayout.ObjectField(casterBundle, typeof(CasterBundle), false) as CasterBundle;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Spell Bundle");
        spellBundle = EditorGUILayout.ObjectField(spellBundle, typeof(SpellWordBundle), false) as SpellWordBundle;
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        if (csvFile)
        {
            string assetPath = AssetDatabase.GetAssetPath(csvFile.GetInstanceID());
            if (assetPath != null && GUILayout.Button("Build CSV Entries", GUILayout.Width(BUTTON_WIDTH)))
            {
                const string assetPathPrefix = "Assets";
                var fullPath = Application.dataPath + assetPath.Substring(assetPathPrefix.Length);
                TIPSBundleParser.Parse(fullPath);
            }
        }

        if (casterBundle && GUILayout.Button("Build Caster Entries", GUILayout.Width(BUTTON_WIDTH)))
        {
            TIPSBundleParser.Parse(casterBundle);
        }

        if (spellBundle && GUILayout.Button("Build Spellword Entries", GUILayout.Width(BUTTON_WIDTH)))
        {
            TIPSBundleParser.Parse(spellBundle);
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Clear All Entries", GUILayout.Width(BUTTON_WIDTH)))
        {
            TIPSBundleParser.ClearAllEntries();
        }

        if (GUILayout.Button("Clear Caster Entries", GUILayout.Width(BUTTON_WIDTH)))
        {
            TIPSBundleParser.ClearCasterEntries();
        }

        if (GUILayout.Button("Clear Spellword Entries", GUILayout.Width(BUTTON_WIDTH)))
        {
            TIPSBundleParser.ClearSpellEntries();
        }
    }
}
