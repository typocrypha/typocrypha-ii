using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Pronoun { FEMININE, INCLUSIVE, FIRSTNAME, MASCULINE };

/// <summary>
/// Container class for player data and progress.
/// Contains several maps for storing data.
/// Game is generally saved during dialog scenes via 'DialogGraphParser'
/// </summary>
public class PlayerDataManager : MonoBehaviour, ISavable
{
    #region ISavable
    [System.Serializable]
    public struct SaveData
    {
        public Dictionary<string, string> stringMap;
        public Dictionary<string, int> intMap;
        public Dictionary<string, float> floatMap;
        public Dictionary<string, bool> boolMap;
    }

    // Only saves strings, ints, floats, and bools.
    public void Save()
    {
        var sd = new SaveData();
        sd.stringMap = new Dictionary<string, string>();
        sd.intMap = new Dictionary<string, int>();
        sd.floatMap = new Dictionary<string, float>();
        sd.boolMap = new Dictionary<string, bool>();

        foreach (var kvp in data)
        {
            if (kvp.Value is string)
                sd.stringMap.Add(kvp.Key, (string)kvp.Value);
            else if (kvp.Value is int)
                sd.intMap.Add(kvp.Key, (int)kvp.Value);
            else if (kvp.Value is float)
                sd.floatMap.Add(kvp.Key, (float)kvp.Value);
            else if (kvp.Value is bool)
                sd.boolMap.Add(kvp.Key, (bool)kvp.Value);
        }

        SaveManager.instance.loaded.playerData = sd;
    }

    public void Load()
    {
        var sd = SaveManager.instance.loaded.playerData;
        SetDefaults();
        foreach (var kvp in sd.stringMap)
            if (!data.ContainsKey(kvp.Key))
                data.Add(kvp.Key, kvp.Value);
        foreach (var kvp in sd.intMap)
            if (!data.ContainsKey(kvp.Key))
                data.Add(kvp.Key, kvp.Value);
        foreach (var kvp in sd.floatMap)
            if (!data.ContainsKey(kvp.Key))
                data.Add(kvp.Key, kvp.Value);
        foreach (var kvp in sd.boolMap)
            if (!data.ContainsKey(kvp.Key))
                data.Add(kvp.Key, kvp.Value);
    }
    #endregion

    public static PlayerDataManager instance = null; // global static ref
    public Dictionary<string, object> data; // string-object data map
    public object this[string key] // Get/set data from internal data maps
    {
        get
        {
            if (data.ContainsKey(key))
            {
                return data[key];
            }
            Debug.LogWarning("PlayerDialogueInfo: no info with key " + key + ", returning null");
            return null;
        }
        set
        {
            if (data.ContainsKey(key))
            {
                data[key] = value;
            }
            else
            {
                data.Add(key, value);
            }
        }
    }

    #region Preset string key constants (faux named map)
    public const string nullKey = "NULL";
    public const string playerName = "player-name";
    public const string lastInputKey = "prompt";

    public const string textDelayScale = "text-delay-scale";
    #endregion

    public PlayerEquipment equipment;
    public ResearchData researchData;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        SetDefaults();
    }

    string target = nullKey;

    /// <summary>
    /// Set default parameters.
    /// </summary>
    public void SetDefaults()
    {
        data = new Dictionary<string, object>
        {
            { playerName, "???" },
            { lastInputKey, "" },
            { textDelayScale, 1f }
        };
    }

    /// <summary>
    /// Set data target (used mainly for input).
    /// Should call 'SetTargetValue' immediately after.
    /// </summary>
    /// <param name="kvp">Key that will be set.</param>
    public void SetTargetKey(string key)
    {
        if (target == nullKey) target = key;
        else Debug.LogWarning("PlayerDialogueInfo: Trying to set in use target. Target is untouched.");
    }

    /// <summary>
    /// Set data target (used mainly for input).
    /// Should be called immediately after 'SetTargetKey'.
    /// </summary>
    /// <param name="value">Value to set.</param>
    void SetTargetValueOBJ(object value)
    {
        this[target] = value;
        target = nullKey;
    }
    public void SetTargetValue(float value)
    {
        SetTargetValueOBJ(value);
    }
    public void SetTargetValue(int value)
    {
        SetTargetValueOBJ(value);
    }
    public void SetTargetValue(string value)
    {
        SetTargetValueOBJ(value);
    }
}
