﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Gameflow;

/// <summary>
/// Starts and manages dialog sequences.
/// </summary>
[RequireComponent(typeof(DialogGraphParser))]
public class DialogManager : MonoBehaviour, IPausable
{
    #region IPausable
    PauseHandle ph;
    public PauseHandle PH { get => ph; }

    public void OnPause(bool b)
    {
        enabled = !b; // Disable input checking.
        if (dialogBox != null) dialogBox.PH.Pause = b; // Pause dialog box scrolling.
        TextEvents.instance.PH.Pause = b; // Pause text events.
    }
    #endregion

    public static DialogManager instance = null;
    public bool startOnAwake = true; // Should dialog start when scene starts up?
    public List<DialogView> allViews; // All dialog views (VN, CHAT, etc)
    public UnityEvent onNextDialog; // Event called when a new dialog line is started.
    public UnityEvent onSkip; // Event called when user manually skips text scroll.
    [HideInInspector] public DialogView dialogView; // Currently displayed dialog view.
    [HideInInspector] public DialogBox dialogBox; // Latest displayed dialog box.

    private DialogGraphParser graphParser; // Dialog graph currently playing.

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

        ph = new PauseHandle(OnPause);
        graphParser = GetComponent<DialogGraphParser>();
        if (startOnAwake)
        {
            StartDialog();
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
    /// Start new dialog graph.
    /// </summary>
    /// <param name="graph">Graph object to start.</param>
    public void StartDialog(DialogCanvas graph)
    {
        gameObject.SetActive(true);
        graphParser.Graph = graph;
        graphParser.Init();
        NextDialog();
    }

    /// <summary>
    /// Start new dialog graph. Implicitly uses graph already in parser.
    /// </summary>
    public void StartDialog()
    {
        graphParser.Init();
        NextDialog();
    }

    /// <summary>
    /// Starts next dialog box.
    /// </summary>
    public void NextDialog()
    {
        DialogItem dialogItem = graphParser.NextDialog();
        if (dialogItem == null) return;
        // Get and display proper view.
        DialogView view = allViews.Find(v => v.GetType() == dialogItem.GetView());
        if (view != dialogView)
        {
            dialogView = view;
            SoloView(dialogView); 
        }
        dialogBox = dialogView.PlayDialog(dialogItem); // Play Dialog
        onNextDialog.Invoke();
    }

    /// <summary>
    /// Show/Hide dialog UI/characters/etc. DOES NOT pause.
    /// </summary>
    /// <param name="show">If true, display dialog. Otherwise, hide.</param>
    public void Display(bool show)
    {
        if (show) transform.position = Vector3.zero;
        else transform.Translate(30f, 0f, 0f);
    }

    // Hide all views except for current.
    void SoloView(DialogView view)
    {
        foreach (var dv in allViews)
        {
            dv.gameObject.SetActive(false);
        }
        view.gameObject.SetActive(true);
    }
}

