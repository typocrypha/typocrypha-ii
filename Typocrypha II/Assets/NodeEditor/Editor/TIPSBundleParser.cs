#if UNITY_EDITOR

using System.IO;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

public class TIPSBundleParser {

    const string TIPS_ROOT_PATH = "ScriptableObjects/TIPS/Root";
    const string TIPS_DEMONS_PATH = "ScriptableObjects/TIPS/Root/Demons";
    const string TIPS_SPELLS_PATH = "ScriptableObjects/TIPS/Root/Spells";

    public static void Parse(string filePath)
    {
        using (var reader = new CsvReader(filePath))
        {
            var entries = new List<TIPSEntryData>();
            bool firstRow = true;

            foreach (string[] values in reader.RowEnumerator)
            {
                if (firstRow)
                {
                    firstRow = false;
                    continue;
                }

                if (values[3] == "") continue; //no name

                string entryPath = string.Join("/", TIPS_ROOT_PATH, values[0], values[1], values[2], $"{values[3]}.asset");
                entryPath = Regex.Replace(entryPath, "/+", "/");

                var parentDirectory = Path.Combine(Application.dataPath, Path.GetDirectoryName(entryPath));
                Directory.CreateDirectory(parentDirectory);

                var entry = ScriptableObject.CreateInstance<TIPSEntryData>();
                entry.Content = values[4];
                AssetDatabase.CreateAsset(entry, "Assets/" + entryPath);
            }
        }
        TIPSBundleLoader.LoadTIPSBundles();
    }

    public static void Parse(CasterBundle bundle)
    {
        var demonsDirectory = Path.Combine(Application.dataPath, TIPS_ROOT_PATH, "Demons");
        Directory.CreateDirectory(demonsDirectory);

        foreach (var pair in bundle.prefabs)
        {
            var entry = ScriptableObject.CreateInstance<TIPSEntryDemon>();
            entry.prefabEnemy = pair.Value;
            var path = string.Join("/", "Assets", TIPS_ROOT_PATH, "Demons", pair.Key + ".asset");
            AssetDatabase.CreateAsset(entry, path);
        }

        TIPSBundleLoader.LoadTIPSBundles();
    }

    public static void Parse(SpellWordBundle bundle)
    {
        SpellWord[] roots = bundle.words.Select(p => p.Value).Where(p => !p.IsSynonym).ToArray();
        SpellWord[] synonyms = bundle.words.Select(p => p.Value).Where(p => p.IsSynonym).ToArray();

        var spellsDirectory = Path.Combine(Application.dataPath, TIPS_ROOT_PATH, "Spells");
        Directory.CreateDirectory(spellsDirectory);

        foreach (var root in roots)
        {
            var entry = ScriptableObject.CreateInstance<TIPSEntrySpell>();
            entry.spell = root;
            var path = string.Join("/", "Assets", TIPS_ROOT_PATH, "Spells", root.name + ".asset");
            AssetDatabase.CreateAsset(entry, path);
        }

        foreach (var synonym in synonyms)
        {
            var entry = ScriptableObject.CreateInstance<TIPSEntrySpell>();
            entry.spell = synonym;
            var path = string.Join("/", "Assets", TIPS_ROOT_PATH, "Spells", synonym.synonymOf.internalName, synonym.name + ".asset");
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            AssetDatabase.CreateAsset(entry, path);
        }

        TIPSBundleLoader.LoadTIPSBundles();
    }

    public static void ClearAllEntries()
    {
        var root = Path.Combine(Application.dataPath, TIPS_ROOT_PATH);
        Directory.Delete(root, true);
        Directory.CreateDirectory(root);
        AssetDatabase.Refresh();
    }

    public static void ClearCasterEntries()
    {
        var root = Path.Combine(Application.dataPath, TIPS_DEMONS_PATH);
        Directory.Delete(root, true);
        Directory.CreateDirectory(root);
        AssetDatabase.Refresh();
    }

    public static void ClearSpellEntries()
    {
        var root = Path.Combine(Application.dataPath, TIPS_SPELLS_PATH);
        Directory.Delete(root, true);
        Directory.CreateDirectory(root);
        AssetDatabase.Refresh();
    }
}

// https://stackoverflow.com/questions/769621/dealing-with-commas-in-a-csv-file

public sealed class CsvReader : System.IDisposable
{
    public CsvReader(string fileName) : this(new FileStream(fileName, FileMode.Open, FileAccess.Read))
    {
    }

    public CsvReader(Stream stream)
    {
        __reader = new StreamReader(stream);
    }

    public System.Collections.IEnumerable RowEnumerator
    {
        get
        {
            if (null == __reader)
                throw new System.ApplicationException("I can't start reading without CSV input.");

            __rowno = 0;
            string sLine;
            string sNextLine;

            while (null != (sLine = __reader.ReadLine()))
            {
                while (rexRunOnLine.IsMatch(sLine) && null != (sNextLine = __reader.ReadLine()))
                    sLine += "\n" + sNextLine;

                __rowno++;
                string[] values = rexCsvSplitter.Split(sLine);

                for (int i = 0; i < values.Length; i++)
                    values[i] = Csv.Unescape(values[i]);

                yield return values;
            }

            __reader.Close();
        }
    }

    public long RowIndex { get { return __rowno; } }

    public void Dispose()
    {
        if (null != __reader) __reader.Dispose();
    }

    //============================================


    private long __rowno = 0;
    private TextReader __reader;
    private static Regex rexCsvSplitter = new Regex(@",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))");
    private static Regex rexRunOnLine = new Regex(@"^[^""]*(?:""[^""]*""[^""]*)*""[^""]*$");
}

public static class Csv
{
    public static string Escape(string s)
    {
        if (s.Contains(QUOTE))
            s = s.Replace(QUOTE, ESCAPED_QUOTE);

        if (s.IndexOfAny(CHARACTERS_THAT_MUST_BE_QUOTED) > -1)
            s = QUOTE + s + QUOTE;

        return s;
    }

    public static string Unescape(string s)
    {
        if (s.StartsWith(QUOTE) && s.EndsWith(QUOTE))
        {
            s = s.Substring(1, s.Length - 2);

            if (s.Contains(ESCAPED_QUOTE))
                s = s.Replace(ESCAPED_QUOTE, QUOTE);
        }

        return s;
    }


    private const string QUOTE = "\"";
    private const string ESCAPED_QUOTE = "\"\"";
    private static char[] CHARACTERS_THAT_MUST_BE_QUOTED = { ',', '"', '\n' };
}

#endif