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

    DialogParser dialogParser; // Parses dialog items

    // DIALOG GRAPH PARSER FIELD 

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
        // GET DIALOG ITEM FROM GRAPH PARSER
        //DialogItem dialogItem = dialogParser.Parse(dialogItem, dialogBox);
    }
}

