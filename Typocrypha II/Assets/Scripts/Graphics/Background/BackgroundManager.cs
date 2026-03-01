using UnityEngine;

/// <summary>
/// Manages the background and background effects.
/// </summary>
public class BackgroundManager : MonoBehaviour
{
    public static BackgroundManager instance = null;
    public SpriteRenderer bgsr; // Sprite renderer for the background.
    public GameObject bggo; // Gameobject background (for more complicated backgrounds).

    public Bounds GetBounds() => bggo ? bggo.GetComponentInChildren<SpriteRenderer>().bounds : bgsr.bounds;

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
        if (bggo.name == prefab.name) return;
        if (bggo != null) Destroy(bggo);
        bggo = Instantiate(prefab, transform);
        bggo.name = prefab.name;
        bgsr.sprite = null;
    }
}
