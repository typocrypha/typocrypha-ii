using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Pronoun { FEMININE, INCLUSIVE, FIRSTNAME, MASCULINE };

/// <summary>
/// Container class for player data and progress.
/// Contains several maps for storing data.
/// Game is generally saved during dialog scenes via 'DialogGraphParser'
/// </summary>
public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager instance = null; // global static ref
    private readonly Dictionary<string, object> data = new Dictionary<string, object>(); // string-object data map

    #region Preset string key constants (faux named map)
    public const string nullKey = "NULL";
    // Gameplay
    public const string lastInputKey = "prompt";
    #endregion

    public PlayerEquipment equipment;
    public ResearchData researchData;
    public ShopData ShopData => shopData;
    [SerializeField] private ShopData shopData;
    public int currency;

    public bool CanCastSpell(SpellWord word)
    {
        return equipment.EquippedSpellWords.ContainsKey(word.Key)
            || (word.IsSynonym && equipment.EquippedSpellWords.ContainsKey(word.synonymOf.Key));// && equipment.UnlockedWords.ContainsKey(word.Key));
    }

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

    /// <summary>
    /// Set default parameters.
    /// </summary>
    public void SetDefaults()
    {
        data.Clear();
        Set(lastInputKey, "");
    }

    public object GetObj(string key)
    {
        if (!data.ContainsKey(key))
        {
            Debug.LogWarning($"PlayerData: no data with key {key}, returning null");
            return null;
        }
        return data[key];
    }

    public T Get<T>(string key)
    {
        if (!data.ContainsKey(key))
        {
            Debug.LogWarning($"PlayerData: no data with key {key}, returning default");
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
}
