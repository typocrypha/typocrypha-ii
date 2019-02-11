using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Starts and manages dialog sequences.
/// </summary>
[RequireComponent(typeof(DialogGraphParser))]
public class DialogManager : MonoBehaviour
{
    public bool startOnAwake = true; // Should dialog start when scene starts up?
    public List<DialogView> allViews; // All dialog views (VN, CHAT, etc)

    private DialogGraphParser graph; // Dialog graph currently playing.
    private DialogBox dialogBox; // Latest displayed dialog box.

    void Awake()
    {
        if (startOnAwake)
        {
            InitDialog();
            NextDialog();
        }
    }

    void Update()
    {
        // Check if submit key is pressed
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (dialogBox.IsDone) // If dialog is done, go to next dialog
            {
                NextDialog();
            }
            else // Otherwise, skip text scroll and dump current text
            {
                dialogBox.DumpText();
            }
        }
    }

    /// <summary>
    /// Initialize dialog parsing (from graph).
    /// </summary>
    public void InitDialog()
    {
        graph = GetComponent<DialogGraphParser>();
        graph.Init();
    }

    /// <summary>
    /// Starts next dialog box.
    /// </summary>
    public void NextDialog()
    {
        DialogItem dialogItem = graph.NextDialog();
        DialogView dialogView = allViews.Find(v => v.GetType() == dialogItem.GetView());
        SoloView(dialogView);
        dialogBox = dialogView.PlayDialog(dialogItem); // Play Dialog
    }

    // Hide all views except for 'view'
    void SoloView(DialogView view)
    {
        foreach (var dv in allViews)
        {
            dv.gameObject.SetActive(false);
        }
        view.gameObject.SetActive(true);
    }
}

