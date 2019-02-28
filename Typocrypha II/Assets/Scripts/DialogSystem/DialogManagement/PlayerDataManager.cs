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
    public Dictionary<string, object> tmpData; // string-object data map
    public object this[string key] // Get/set data from internal data maps
    {
        get
        {
            if (tmpData.ContainsKey(key))
            {
                return tmpData[key];
            }
            Debug.LogWarning("PlayerDialogueInfo: no info with key " + key + ", returning null");
            return null;
        }
        set
        {
            if (tmpData.ContainsKey(key))
            {
                tmpData[key] = value;
            }
            else
            {
                tmpData.Add(key, value);
            }
        }
    }

    #region Preset string key constants (faux named map)
    public const string nullKey = "NULL";
    public const string playerName = "player-name";
    public const string lastInputKey = "prompt";

    public const string textDelayScale = "text-delay-scale";
    #endregion

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

        tmpData = new Dictionary<string, object>
        {
            { playerName, "???" },
            { lastInputKey, "" },
            { textDelayScale, 1f }
        };
    }

    string target = nullKey;

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
