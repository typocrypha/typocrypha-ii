using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using System.Text;
// Made with the help of Thomas Bryant during his time at UCSC.

/// <summary>
/// Container for actual save data.
/// Namely, write data to 'SaveManager.instance.loaded'.
/// </summary>
[System.Serializable]
public class CampaignSaveData
{
    public string currentSceneName;
    public int currentSceneIndex;
    public List<string> equippedBadgeWords = new List<string>();
}

[System.Serializable]
public class GlobalSaveData
{
    public List<string> unlockedSpellWords = new List<string>();
    public List<string> unlockedBadgeWords = new List<string>();
}

/// <summary>
/// Manages creating save files, loading saves, and deleting saves.
/// </summary>
public class SaveManager : MonoBehaviour
{
    public static SaveManager instance = null; // Global static reference
    private static string SaveFilePath(int saveIndex) => Path.Combine(Application.persistentDataPath, $"campaignSaveData{saveIndex}.dat");
    private static string GlobalSaveFilePath() => Path.Combine(Application.persistentDataPath, "globalSaveData.dat");

    private int loadedCampaignIndex = 0;

    void Awake()
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
    }

    private void Start()
    {
        LoadGlobalData();
    }

    private static void SaveFile(object saveData, string path)
    {
        try
        {
            //convert to JSON, then to bytes
            string jsonData = JsonUtility.ToJson(saveData, true);
            byte[] jsonByte = Encoding.ASCII.GetBytes(jsonData);

            //create the save directory if it doesn't exist
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }
            File.WriteAllBytes(path, jsonByte);
        }
        catch(Exception e)
        {
            Debug.LogError($"Error saving file: {e}");
        }
    }

    private static T LoadFile<T>(string path) where T : new()
    {
        if (File.Exists(path))
        {
            try
            {
                var data = new T();
                string jsonData = Encoding.ASCII.GetString(File.ReadAllBytes(path));
                JsonUtility.FromJsonOverwrite(jsonData, data);
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading file: {e}");
            }
        }
        return default;
    }

    public bool HasCampaignSaveFile(int saveIndex = 0)
    {
        return File.Exists(SaveFilePath(saveIndex));
    }

    public bool HasGlobalSaveFile() => File.Exists(GlobalSaveFilePath());

    /// <summary>
    /// Creates new save game file.
    /// </summary>
    /// <param name="saveIndex">Save slot index.</param>
    public void NewGame()
    {
        SaveFile(new CampaignSaveData(), SaveFilePath(0));
    }

    public void Save()
    {
        SaveCampaign();
        SaveGlobalData();
    }

    /// <summary>
    /// Save the currently loaded game data into the save file.
    /// </summary>
    public void SaveCampaign() => SaveCampaign(loadedCampaignIndex);

    private void SaveCampaign(int saveIndex)
    {
        SaveFile(GetCampaignSaveData(), SaveFilePath(saveIndex));
    }

    private CampaignSaveData GetCampaignSaveData()
    {
        var data = new CampaignSaveData();
        var transitionManager = TransitionManager.instance;
        data.currentSceneIndex = transitionManager.SceneIndex;
        data.currentSceneName = transitionManager.SceneName;
        var dataManager = PlayerDataManager.instance;
        var equipment = dataManager.equipment;
        // Get Unlocked Badges
        foreach (var kvp in equipment.EquippedBadgeWords)
        {
            var badge = kvp.Value;
            data.equippedBadgeWords.Add(badge.Key);
        }
        return data;
    }

    public void LoadCampaign(int saveIndex = 0)
    {
        loadedCampaignIndex = saveIndex;
        var data = LoadFile<CampaignSaveData>(SaveFilePath(saveIndex));
        LoadCampaignData(data);
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public void DebugLoadCampaign()
    {
        const int debugSaveFile = 99;
        if (!HasCampaignSaveFile(debugSaveFile))
        {
            SaveCampaign(debugSaveFile);
        }
        LoadCampaign(debugSaveFile);
        PlayerDataManager.instance.equipment.ReapplyEquippedBadgeWords(Battlefield.instance.Player);
    }

    private void LoadCampaignData(CampaignSaveData data)
    {
        TransitionManager.instance.LoadIndex(data.currentSceneName, data.currentSceneIndex);
        var dataManager = PlayerDataManager.instance;
        var equipment = dataManager.equipment;
        // Equipped badges
        equipment.ClearEquippedBadges();
        foreach (var key in data.equippedBadgeWords)
        {
            if (Lookup.TryGetBadge(key, out var badge))
            {
                equipment.EquipBadge(badge);
            }
        }
    }

    public void SaveGlobalData()
    {
        SaveFile(GetGlobalSaveData(), GlobalSaveFilePath());
    }

    private GlobalSaveData GetGlobalSaveData()
    {
        var dataManager = PlayerDataManager.instance;
        var equipment = dataManager.equipment;
        // Get Unlocked Spell Words
        var data = new GlobalSaveData();
        foreach(var kvp in equipment.UnlockedSpellWords)
        {
            var word = kvp.Value;
            if (word.IsDebug)
                continue;
            data.unlockedSpellWords.Add(word.Key);
        }
        // Get Unlocked Badges
        foreach(var kvp in equipment.UnlockedBadgeWords)
        {
            var badge = kvp.Value;
            data.unlockedBadgeWords.Add(badge.Key);
        }
        return data;
    }

    public void LoadGlobalData()
    {
        if (!HasGlobalSaveFile())
        {
            SaveFile(new GlobalSaveData(), GlobalSaveFilePath());
        }
        LoadGlobalData(LoadFile<GlobalSaveData>(GlobalSaveFilePath()));
    }

    private void LoadGlobalData(GlobalSaveData data)
    {
        var dataManager = PlayerDataManager.instance;
        var equipment = dataManager.equipment;
        // Unlocked spells
        equipment.ClearUnlockedSpells();
        foreach(var key in data.unlockedSpellWords)
        {
            if(Lookup.TryGetSpellWord(key, out var word))
            {
                equipment.UnlockWord(word);
            }
        }
        // Unlocked badges
        equipment.ClearUnlockedBadges();
        foreach(var key in data.unlockedBadgeWords)
        {
            if (Lookup.TryGetBadge(key, out var badge))
            {
                equipment.UnlockBadge(badge);
            }
        }
    }
}
