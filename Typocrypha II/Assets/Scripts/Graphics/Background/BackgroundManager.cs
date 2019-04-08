using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Manages the background and background effects.
/// </summary>
public class BackgroundManager : MonoBehaviour, ISavable
{
    #region ISavable
    public void Save()
    {
        // TEMP
        SaveManager.instance.loaded.bgsprite = bgsr.sprite.name; 
    }

    public void Load()
    {
        // TEMP
        AssetBundle ab = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "sprite"));
        bgsr.sprite = ab.LoadAsset<Sprite>(SaveManager.instance.loaded.bgsprite);
        ab.Unload(false);
    }
    #endregion

    public static BackgroundManager instance = null;
    public SpriteRenderer bgsr; // Sprite renderer for the background.
    public GameObject bggo; // Gameobject background (for more complicated backgrounds).

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
    /// Set background sprite.
    /// </summary>
    /// <param name="sprite">Sprite to set as background.</param>
    public void SetBackground(Sprite sprite)
    {
        if (bggo != null) Destroy(bggo);
        bgsr.sprite = sprite;
    }

    /// <summary>
    /// Set background with a prefab.
    /// </summary>
    /// <param name="prefab">Prefab of background object.</param>
    public void SetBackground(GameObject prefab)
    {
        if (bggo != null) Destroy(bggo);
        bggo = Instantiate(prefab, transform);
        bgsr.sprite = null;
    }
}
