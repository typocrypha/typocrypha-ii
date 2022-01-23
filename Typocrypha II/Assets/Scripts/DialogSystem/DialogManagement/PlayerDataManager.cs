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
    private readonly Dictionary<string, object> data = new Dictionary<string, object>(); // string-object data map

    #region Preset string key constants (faux named map)
    public const string nullKey = "NULL";
    public const string playerName = "player-name";
    public const string lastInputKey = "prompt";

    public const string textDelayScale = "text-delay-scale";
    public const string mainCharacterName = "mc-name";
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
        data.Clear();
        Set(playerName, "???");
        Set(lastInputKey, "");
        Set(textDelayScale, 30f);
        Set(mainCharacterName, "Ayin");
    }

    public object GetObj(string key)
    {
        if (!data.ContainsKey(key))
        {
            Debug.LogWarning("PlayerDialogueInfo: no info with key " + key + ", returning null");
            return null;
        }
        return data[key];
    }

    public T Get<T>(string key)
    {
        if (!data.ContainsKey(key))
        {
            Debug.LogWarning("PlayerDialogueInfo: no info with key " + key + ", returning default");
            return default;
        }
        return (T)data[key];
    }

    public void Set(string key, object obj)
    {
        if (data.ContainsKey(key))
        {
            data[key] = obj;
            return;
        }
        data.Add(key, obj);
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
        Set(target, value);
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
