using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Menu behavior for equipping spells before battle.
/// </summary>
public class EquipMenu : MonoBehaviour
{
    public string WordBundleName; // Name of Asset bundle with all valid words.
    // IMPLEMENT WAY TO LOCK/UNLOCK WORDS
    public GameObject ItemPrefab; // Prefab for word item in menu.
    public RectTransform WordPanel; // Panel for holding word items.

    AssetBundle WordBundle; // Asset bundle with word objects.

    private void Awake()
    {
        if (WordBundle == null)
        {
            WordBundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(Application.streamingAssetsPath, WordBundleName));
        }
    }

    /// <summary>
    /// Add word to equipment panel.
    /// </summary>
    /// <param name="word"></param>
    public void EquipWord(string word)
    {
        if (WordBundle.Contains(word))
        {
            var go = Instantiate(ItemPrefab, WordPanel);
            go.GetComponentInChildren<UnityEngine.UI.Text>().text = word;
        }
    }
}
