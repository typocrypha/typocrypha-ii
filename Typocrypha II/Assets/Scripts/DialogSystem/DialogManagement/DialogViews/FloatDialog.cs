using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages floating text during dialog.
/// </summary>
public class FloatDialog : MonoBehaviour
{
    public static FloatDialog instance = null;
    public GameObject dialogBoxFloatPrefab; // Prefab for floating dialog box.

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
    }

    /// <summary>
    /// Spawn a float text dialog box.
    /// </summary>
    /// <param name="line">Text to display.</param>
    /// <param name="pos">Position to spawn.</param>
    public void SpawnFloatDialog(string line, Vector2 pos)
    {
        var go = Instantiate(dialogBoxFloatPrefab, transform);
        go.transform.position = pos;
        var fdb = go.GetComponent<DialogBoxFloat>();
        fdb.StartDialogBox(new DialogItemFloat(line));
    }
}
