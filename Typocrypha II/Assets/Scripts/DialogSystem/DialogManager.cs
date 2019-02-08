using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Starts and manages dialog sequences.
/// </summary>
[RequireComponent(typeof(DialogGraphParser))]
public class DialogManager : MonoBehaviour
{
    public bool startOnAwake = true; // Should dialog start when scene starts up?
    private DialogGraphParser graph;

    // DIALOG GRAPH PARSER FIELD 

    void Awake()
    {
        if (startOnAwake)
        {
            graph = GetComponent<DialogGraphParser>();
            graph.Init();
            NextDialog();
        }
    }

    void Update()
    {
        // CHECK NEXT DIALOG INPUT
    }

    /// <summary>
    /// Starts next dialog box.
    /// </summary>
    public void NextDialog()
    {
        DialogView dialogView = null; // FIGURE OUT VIEW
        DialogItem dialogItem = graph.NextDialog();
        DialogBox dialogBox = dialogView.PlayDialog(dialogItem); // Play Dialog
    }
}

