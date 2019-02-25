using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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
        dialogBox.PH.Pause = b; // Pause dialog box scrolling.
        TextEvents.instance.PH.Pause = b; // Pause text events.
    }
    #endregion

    public static DialogManager instance = null;
    public bool startOnAwake = true; // Should dialog start when scene starts up?
    public List<DialogView> allViews; // All dialog views (VN, CHAT, etc)
    [HideInInspector] public DialogView dialogView; // Currently displayed dialog view.
    [HideInInspector] public DialogBox dialogBox; // Latest displayed dialog box.

    private DialogGraphParser graph; // Dialog graph currently playing.

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
        // Get and display proper view.
        DialogView view = allViews.Find(v => v.GetType() == dialogItem.GetView());
        if (view != dialogView)
        {
            dialogView = view;
            SoloView(dialogView); 
        }
        dialogBox = dialogView.PlayDialog(dialogItem); // Play Dialog
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

