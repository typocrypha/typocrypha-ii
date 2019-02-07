using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Starts and manages dialog sequences.
/// </summary>
[RequireComponent(typeof(DialogParser))]
public class DialogManager : MonoBehaviour
{
    public bool startOnAwake = true; // Should dialog start when scene starts up?
    public DialogBox dialogBoxPrefab; // Prefab of base dialog box.

    DialogParser dialogParser; // Parses graph dialog into dialog items.

    // DIALOG GRAPH FIELD 

    void Awake()
    {
        dialogParser = GetComponent<DialogParser>();
        if (startOnAwake)
        {
            NextDialog();
        }
    }

    /// <summary>
    /// Starts next dialog box.
    /// </summary>
    public void NextDialog()
    {
        // FIGURE OUT VIEW
        DialogBox dialogBox = Instantiate(dialogBoxPrefab);
        DialogItem dialogItem = dialogParser.Parse(dialogBox);
    }
}

