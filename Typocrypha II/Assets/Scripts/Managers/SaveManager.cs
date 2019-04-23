using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
// Made with the help of Thomas Bryant during his time at UCSC.

/// <summary>
/// Container for actual save data.
/// Namely, write data to 'SaveManager.instance.loaded'.
/// </summary>
[System.Serializable]
public struct GameData
{
    public int saveIndex; // Index of save (save slot number).
    public string currScene; // Scene that player was in.

    public PlayerDataManager.SaveData playerData;
    public int nodeCount; // TEMP: how many dialog nodes deep player was in.
    public string bgsprite;
    public string bgm;
    public List<DialogCharacterManager.CharacterSave> characters;

    /// <summary>
    /// Set default (new game) values.
    /// </summary>
    public void SetNewGameDefaults()
    {
        currScene = "newgame";
        nodeCount = 0;
    }
}

/// <summary>
/// Manages creating save files, loading saves, and deleting saves.
/// </summary>
public class SaveManager : MonoBehaviour
{
    public static SaveManager instance = null; // Global static reference
    public GameData loaded; // Loaded game data 

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

    /// <summary>
    /// Creates new save game file.
    /// </summary>
    /// <param name="saveIndex">Save slot index.</param>
    public void NewGame(int saveIndex)
    {
        loaded = new GameData();
        loaded.SetNewGameDefaults();
        loaded.saveIndex = saveIndex;

        Debug.Log("making new save:" + Application.persistentDataPath + "/savefile" + saveIndex + ".dat");
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

        foreach (var savable in GameObject.FindGameObjectsWithTag("Savable")) savable.GetComponent<ISavable>().Save();

        Debug.Log("saving to:" + Application.persistentDataPath + "/savefile" + loaded.saveIndex + ".dat");
        FileStream file = File.Open(Application.persistentDataPath + "/savefile" + loaded.saveIndex + ".dat", FileMode.Open);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, loaded);
        file.Close();
    }

    /// <summary>
    /// Load the parameters saved into the save file.
    /// </summary>
    /// <param name="saveIndex">Save slot index.</param>
    public void LoadGame(int saveIndex)
    {
        if (File.Exists(Application.persistentDataPath + "/savefile" + saveIndex + ".dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savefile" + saveIndex + ".dat", FileMode.Open);
            loaded = (GameData)bf.Deserialize(file);
            file.Close();
        }
    }

    /// <summary>
    /// Apply loaded state to scene.
    /// </summary>
    public void ApplyState()
    {
        foreach (var savable in GameObject.FindGameObjectsWithTag("Savable")) savable.GetComponent<ISavable>().Load();
    }
}
