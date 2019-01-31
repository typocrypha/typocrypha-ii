using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
// Made with the help of Thomas Bryant during his time at UCSC

/// <summary>
/// Container for actual save data. Put all data to be saved here.
/// </summary>
[Serializable]
public struct GameData
{
    public int saveIndex; // Index of save (save slot number)

    /// <summary>
    /// Set default (new game) values.
    /// </summary>
    public void SetNewGame()
    {
    }
}

/// <summary>
/// Manages creating save files, loading saves, and deleting saves.
/// </summary>
public class SaveManager : MonoBehaviour
{
    public static SaveManager instance = null; // Global static reference
    public GameData loaded; // Loaded game data 

    void Awake() {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Creates new save game file.
    /// </summary>
    /// <param name="saveIndex">Save slot index.</param>
    public void NewGame(int saveIndex)
    {
        loaded = new GameData();
        loaded.SetNewGame();
        loaded.saveIndex = saveIndex;
        
        FileStream file = new FileStream(Application.persistentDataPath + "/savefile" + saveIndex + ".dat", FileMode.Create);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, loaded);
        file.Close();
    }

    /// <summary>
    /// Save the currently loaded game data into the save file.
    /// </summary>
    public void SaveGame()
    {
        FileStream file = File.Open(Application.persistentDataPath + "/savefile" + loaded.saveIndex + ".dat", FileMode.Open);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, loaded);
        file.Close();
    }

    /// <summary>
    /// Load the parameters saved into the save file.
    /// </summary>
    /// <param name="saveIndex">Save slot index.</param>
    public void LoadGame(int saveIndex) {
        if (File.Exists(Application.persistentDataPath + "/savefile" + saveIndex + ".dat")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savefile" + saveIndex + ".dat", FileMode.Open);
            loaded = (GameData)bf.Deserialize(file);
            file.Close();
        }
    }
}
