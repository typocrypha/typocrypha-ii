using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Gameflow;

/// <summary>
/// Starts and manages dialog sequences.
/// </summary>
[RequireComponent(typeof(DialogGraphParser))]
public class DialogManager : MonoBehaviour, IPausable, ISavable
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

    #region ISavable
    public void Save()
    {
        SaveManager.instance.loaded.currScene = SceneManager.GetActiveScene().name;
        SaveManager.instance.loaded.nodeCount = dialogCounter;
    }

    public void Load()
    {
        dialogCounter = SaveManager.instance.loaded.nodeCount;
    }
    #endregion

    public static DialogManager instance = null;
    public bool startOnStart = true; // Should dialog start when scene starts up? (should generally only be true for debugging)
    public bool isBattle = false; // Is this a battle scene?
    public List<DialogView> allViews; // All dialog views (VN, CHAT, etc)
    public UnityEvent onNextDialog; // Event called when a new dialog line is started.
    public UnityEvent onSkip; // Event called when user manually skips text scroll.
    [HideInInspector] public DialogView dialogView; // Currently displayed dialog view.
    [HideInInspector] public IDialogBox dialogBox; // Latest displayed dialog box.
    [HideInInspector] public int dialogCounter; // Number of dialog lines passed.

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
        if (isBattle) Display(false);
    }

    void Start()
    {
        // Set so that dialog waits for transition
        TransitionManager.instance.onStartScene.AddListener(StartDialog);
        if (startOnStart) StartDialog();
    }

    void Update()
    {
        // Check if submit key is pressed
        if (dialogBox != null && Input.GetKeyDown(KeyCode.Space))
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
    /// May load save if applicable.
    /// </summary>
    /// <param name="graph">Graph object to start.</param>
    public void StartDialog(DialogCanvas graph)
    {
        graphParser.Graph = graph;
        StartDialog();
    }

    /// <summary>
    /// Start new dialog graph. Implicitly uses graph already in parser.
    /// </summary>
    public void StartDialog()
    {
        graphParser.Init();
        if (dialogCounter <= 0) // Start from beginning of scene if no save file load.
        {
            dialogCounter = -1;
            NextDialog();
        }
        else // Otherwise, go to saved position.
        {
            graphParser.FastForward(dialogCounter);
            NextDialog(false);
        }
    }

    /// <summary>
    /// Starts next dialog box.
    /// </summary>
    /// <param name="next">Should we immediately go to next line?
    /// i.e. if false, use current value of 'currNode' in 'DialogGraphParser'.</param>
    public void NextDialog(bool next = true)
    {
        DialogItem dialogItem = graphParser.NextDialog(next);
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

        dialogCounter++;
    }

    /// <summary>
    /// Show/Hide dialog UI/characters/etc. DOES NOT pause.
    /// </summary>
    /// <param name="show">If true, display dialog. Otherwise, hide.</param>
    public void Display(bool show)
    {
        if (show)
        {
            transform.position = Vector3.zero;
            PH.Pause = false;
        }
        else
        {
            transform.Translate(30f, 0f, 0f);
            PH.Pause = true;
        }
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

