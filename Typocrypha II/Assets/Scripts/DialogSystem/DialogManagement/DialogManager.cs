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
    [SerializeField] private List<DialogView> allViews; // All dialog views (VN, CHAT, etc)
    public UnityEvent onNextDialog; // Event called when a new dialog line is started.
    public UnityEvent onSkip; // Event called when user manually skips text scroll.
    public DialogView DialogView
    {
        get => dialogView;
        private set
        {
            if(dialogView == value) return;
            lastView = dialogView;
            dialogView = value;
        }
    }
    private DialogView dialogView; // Currently displayed dialog view.
    private DialogView lastView; // Previously displayed dialog view.
    [HideInInspector] public IDialogBox dialogBox; // Latest displayed dialog box.
    [HideInInspector] public int dialogCounter = 0; // Number of dialog lines passed.

    private bool readyToContinue = true;
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
        SetDefaultView();
    }

    void Start()
    {
        if (startOnStart) StartDialog();
    }

    void Update()
    {
#if DEBUG
        if (!isBattle && Input.GetKeyDown(KeyCode.S))
        {
            StartDialog();
        }
#endif
        // Check if submit key is pressed
        if (readyToContinue && dialogBox != null && DialogView.ReadyToContinue && Input.GetKeyDown(KeyCode.Space))
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

    private void SetDefaultView()
    {
        DialogView = GetView<DialogViewVNPlus>(); // Set to VN plus by default
    }

    /// <summary>
    /// Start new dialog graph.
    /// May load save if applicable.
    /// </summary>
    /// <param name="graph">Graph object to start.</param>
    public void StartDialog(DialogCanvas graph, bool reset = false)
    {
        graphParser.Graph = graph;
        StartDialog(reset);
    }

    /// <summary>
    /// Start new dialog graph. Implicitly uses graph already in parser.
    /// </summary>
    private void StartDialog(bool reset = false)
    {
        PH.Pause = false;
        SetDefaultView();
        graphParser.Init();
        if (reset)
        {
            ResetDialog();
        }
        if (isBattle || dialogCounter <= 0) // Start from beginning of scene if no save file load (can't save in middle of battle).
        {
            dialogCounter = -1;
            NextDialog(true);
        }
        else // Otherwise, go to saved position.
        {
            graphParser.SkipTo(dialogCounter);
            NextDialog(false);
        }
    }

    private void ResetDialog()
    {
        dialogCounter = 0;
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
        // Remove certain old text effects from previous box
        if ((dialogBox as DialogBox) != null) DisableOldTextEffects(dialogBox); 
        // Get and display proper view.
        DialogView view = GetView(dialogItem.GetView());
        if (view != DialogView)
        {
            DialogView = view;
            StartCoroutine(ChangeViews(() => PlayNextDialog(dialogItem)));
        }
        else if (DialogView.IsHidden)
        {
            if (lastView == null || lastView.IsHidden)
            {
                Show(() => PlayNextDialog(dialogItem));
            }
            else
            {
                StartCoroutine(ChangeViews(() => PlayNextDialog(dialogItem)));
            }
        }
        else
        {
            PlayNextDialog(dialogItem);
        }
    }

    private void PlayNextDialog(DialogItem dialogItem)
    {
        dialogBox = DialogView.PlayDialog(dialogItem); // Play Dialog
        onNextDialog.Invoke();
        dialogCounter++;
    }

    /// <summary>
    /// Disable certain text effects on dialog box to reduce visual noise
    /// </summary>
    /// <param name="box">Box to disable text effects</param>
    void DisableOldTextEffects(IDialogBox box)
    {
        var dbox = box as DialogBox;
        var effects = dbox.GetComponents<FXText.TMProEffect>();
        // Disable all movement based effects
        foreach (var effect in effects)
        {
            if (effect.PriorityGroup == FXText.TMProEffect.PriorityGroupEnum.POSITION)
            {
                effect.ind.Clear();
            }
            effect.done = true;
        }
    }

    private DialogView GetView<T>() where T : DialogView
    {
        return GetView(typeof(T));
    }

    private DialogView GetView(System.Type type)
    {
        return allViews.Find(v => v.GetType() == type);
    }

    /// <summary>
    /// Show/Hide dialog UI/characters/etc.
    /// </summary>
    /// <param name="show">If true, display dialog. Otherwise, hide.</param>
    public void Show(System.Action onComplete)
    {
        PH.Pause = false;
        if (DialogView == null || !DialogView.IsHidden)
        {
            onComplete?.Invoke();
            return;
        }
        StartCoroutine(ShowHideView(true, onComplete));
    }

    public void Hide(System.Action onComplete)
    {
        PH.Pause = true;
        if (DialogView == null || DialogView.IsHidden)
        {
            onComplete?.Invoke();
            return;
        }
        StartCoroutine(ShowHideView(false, onComplete));
    }

    private IEnumerator ShowHideView(bool show, System.Action onComplete)
    {
        readyToContinue = false;
        if (show)
        {
            DialogView.gameObject.SetActive(true);
            yield return DialogView.PlayEnterAnimation();
        }
        else
        {
            yield return DialogView.PlayExitAnimation();
            DialogView.gameObject.SetActive(false);
        }
        readyToContinue = true;
        onComplete?.Invoke();
    }

    /// <summary>
    /// Cleans up all dialog views (e.g. deletes old dialog boxes).
    /// </summary>
    public void CleanUp()
    {
        foreach (var view in allViews) view.CleanUp();
    }

    public Coroutine SetView(System.Type viewType)
    {
        var view = GetView(viewType);
        DialogView = view;
        if (view.ShowImmediately)
        {
            return StartCoroutine(ChangeViews(null));
        }
        return null;
    }

    // Hide all views except for current.
    private IEnumerator ChangeViews(System.Action callback)
    {
        readyToContinue = false;
        if (lastView != null && !lastView.IsHidden)
        {
            yield return lastView.PlayExitAnimation();
            lastView.gameObject.SetActive(false);
        }
        dialogView.gameObject.SetActive(true);
        yield return dialogView.PlayEnterAnimation();
        readyToContinue = true;
        callback?.Invoke();
    }
}

