using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Pronoun { FEMININE, INCLUSIVE, FIRSTNAME, MASCULINE };

/// <summary>
/// Container class for player data and progress.
/// Contains several maps for storing data.
/// </summary>
public class PlayerDataManager : MonoBehaviour
{
	public static PlayerDataManager instance = null; // global static ref
    public Dictionary<string, string> tmpData; // Temporary pure string-string map

    public string PlayerName { get { return GetData("player-name"); } set {SetData("player-name", value);} }
    private const string lastInputKey = "prompt";
    public string LastPlayerInput { get { return GetData(lastInputKey); } set { SetData(lastInputKey, value); } }
	public Pronoun player_pronoun { get; set; }

    private void Awake()
    {
        tmpData = new Dictionary<string, string> { };
        tmpData.Add("player-name", "???");
        tmpData.Add(lastInputKey, "");
    }

    void Start() {
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

    // return an info string from the info map, or throw an exception if one does not exist
    public string GetData(string key)
    {
        if (tmpData.ContainsKey(key))
        {
            return tmpData[key];
        }
        Debug.LogWarning("PlayerDialogueInfo: no info with key " + key + ", returning undefined");
        return "undefined";
    }

    // set an info string in the info map
    public void SetData(string key, string data)
    {
        if (tmpData.ContainsKey(key))
        {
            tmpData[key] = data;
        }
        else
        {
            tmpData.Add(key, data);
        }
    }
}
