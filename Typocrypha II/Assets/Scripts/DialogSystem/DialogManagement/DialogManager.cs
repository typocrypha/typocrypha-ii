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
public class DialogManager : MonoBehaviour, IPausable
{
    #region IPausable
    PauseHandle ph;
    public PauseHandle PH { get => ph; }

    public void OnPause(bool b)
    {
        enabled = !b; // Disable input checking.
        if (ActiveDialogBox != null) ActiveDialogBox.PH.SimpleParentPause(b); // Pause dialog box scrolling.
        TextEvents.instance.PH.SimpleParentPause(b); // Pause text events.
    }
    #endregion

    public enum EndType
    {
        None,
        DialogEnd,
        SceneEnd,
    }

    public static DialogManager instance = null;

    public event System.Action OnHideComplete;
    public bool Auto { get; private set; }
    public DialogBox ActiveDialogBox { get; private set; } // Latest displayed dialog box.
    public IReadOnlyList<string> ActiveTIPsEntries
    {
        get
        {
            if (ActiveDialogBox == null)
                return System.Array.Empty<string>();
            return ActiveDialogBox.TIPsEntries;
        }
    }
    public int DialogCounter { get; private set; } = 0; // Number of dialog lines passed.
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
    public bool ReadyToContinue { get; set; } = true;
    public string LocationText
    {
        get => location;
        set
        {
            location = value;
            if(DialogView != null)
            {
                DialogView.SetLocationText(value);
            }
            else
            {
                Debug.LogError($"Attempted to set location text to {value} but there is no active dialog view");
            }
        }
    }
    private string location = "";

    public bool Loading { get; set; } = false;
    public bool IsLoading() => Loading;

    [SerializeField] private bool startOnStart = true; // Should dialog start when scene starts up? (should generally only be true for debugging)
    [SerializeField] private bool isBattle = false; // Is this a battle scene?
    [SerializeField] private List<DialogView> allViews; // All dialog views (VN, CHAT, etc)
    [SerializeField] private UnityEvent onNextDialog; // Event called when a new dialog line is started.
    [SerializeField] UnityEvent onSkip; // Event called when user manually skips text scroll.
    [SerializeField] private DialogGraphParser graphParser;

    private DialogView dialogView; // Currently displayed dialog view.
    private DialogView lastView; // Previously displayed dialog view.
    private float skipTime;

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
        foreach (var view in allViews)
        {
            view.Initialize();
        }
    }

#if DEBUG
    void Start()
    {
        if (startOnStart)
        {
            StartDialog(false);
        }
    }
#endif

    void Update()
    {
        if (Auto)
        {
            return;
        }
        if (Loading || !ReadyToContinue || ActiveDialogBox == null || DialogView == null || !DialogView.ReadyToContinue)
            return;
        if (Input.GetKey(KeyCode.Space) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))) // Fast-forward
        {
            skipTime += Time.deltaTime;
            if(skipTime > 0.033f)
            {
                skipTime = 0;
                if (ActiveDialogBox.IsDone)
                {
                    NextDialog(true); // If dialog is done, go to next dialog
                }
                else
                {
                    ActiveDialogBox.DumpText(); // Otherwise, skip text scroll and dump current text
                }
            }

        }
        else if ((Input.GetKeyDown(KeyCode.Space) || Settings.AutoContinue)) // Normal continue
        {
            skipTime = 0;
            if (ActiveDialogBox.IsDone)
            {
                NextDialog(true); // If dialog is done, go to next dialog
            }
            else if (!Settings.AutoContinue)
            {
                ActiveDialogBox.DumpText(); // Otherwise, skip text scroll and dump current text
            }
        }
    }

    // Starts the dialog, but only allows nodes to execute if they have executeduringloading set to true
    public void LoadDialog(DialogCanvas graph, bool reset)
    {
        Loading = true;
        graphParser.Graph = graph;
        StartDialog(reset);
    }

    /// <summary>
    /// Start new dialog graph.
    /// May load save if applicable.
    /// </summary>
    /// <param name="graph">Graph object to start.</param>
    public void StartDialog(DialogCanvas graph, bool reset, bool autoDialog, System.Action onHideComplete = null)
    {
        Auto = autoDialog;
        graphParser.Graph = graph;
        if(onHideComplete != null)
        {
            OnHideComplete -= onHideComplete;
            OnHideComplete += onHideComplete;
        }
        StartDialog(reset);
    }

    /// <summary>
    /// Start new dialog graph. Implicitly uses graph already in parser.
    /// </summary>
    private void StartDialog(bool reset)
    {
        PH.Unpause(PauseSources.Self);
        if (isBattle && !Auto)
        {
            BattleManager.instance.PH.Pause(PauseSources.Dialog);
        }
        graphParser.Init();
        if (reset)
        {
            ResetDialog();
        }
        if (isBattle || DialogCounter <= 0) // Start from beginning of scene if no save file load (can't save in middle of battle).
        {
            DialogCounter = -1;
            NextDialog(true);
        }
        else // Otherwise, go to saved position.
        {
            graphParser.SkipTo(DialogCounter);
            NextDialog(false);
        }
    }

    private void ResetDialog()
    {
        DialogCounter = 0;
        if(DialogView != null)
        {
            HideViewInstant();
        }
        DialogView = null;
        lastView = null;
    }

    /// <summary>
    /// Starts next dialog box.
    /// </summary>
    /// <param name="next">Should we immediately go to next line?
    /// i.e. if false, use current value of 'currNode' in 'DialogGraphParser'.</param>
    public void NextDialog(bool next)
    {
        DialogItem dialogItem = graphParser.NextDialog(next);
        if (dialogItem == null) return;
        // Remove certain old text effects from previous box
        DisableOldTextEffects(ActiveDialogBox); 
        // Get and display proper view.
        var view = GetView(dialogItem.GetView());
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
        ActiveDialogBox = DialogView.PlayDialog(dialogItem); // Play Dialog
        onNextDialog.Invoke();
        DialogCounter++;
    }

    /// <summary>
    /// Disable certain text effects on dialog box to reduce visual noise
    /// </summary>
    /// <param name="box">Box to disable text effects</param>
    private void DisableOldTextEffects(DialogBox box)
    {
        if (box == null)
            return;
        var effects = box.GetComponents<FXText.TMProEffect>();
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
        if (type == null)
            return DialogView;
        return allViews.Find(v => v.GetType() == type);
    }

    /// <summary>
    /// Show/Hide dialog UI/characters/etc.
    /// </summary>
    /// <param name="show">If true, display dialog. Otherwise, hide.</param>
    public void Show(System.Action onComplete)
    {
        PH.Unpause(PauseSources.Self);
        if (DialogView == null || !DialogView.IsHidden)
        {
            onComplete?.Invoke();
            return;
        }
        StartCoroutine(ShowView(false, onComplete));
    }

    public void Hide(EndType endType, System.Action onComplete)
    {
        if (DialogView == null || DialogView.IsHidden)
        {
            PH.Pause(PauseSources.Self);
            onComplete?.Invoke();
            OnHideComplete?.Invoke();
            OnHideComplete = null;
            return;
        }
        StartCoroutine(HideView(endType, onComplete));
    }

    private IEnumerator ShowView(bool firstView, System.Action onComplete)
    {
        ReadyToContinue = false;
        if (ShouldHideAllyBox)
        {
            yield return StartCoroutine(HideAllyBoxCr());
        }
        DialogView.SetLocationText(LocationText);
        DialogView.gameObject.SetActive(true);
        yield return DialogView.PlayEnterAnimation(firstView);
        ReadyToContinue = true;
        onComplete?.Invoke();
    }
    private IEnumerator HideView(EndType endType, System.Action onComplete)
    {
        ReadyToContinue = false;
        yield return DialogView.PlayExitAnimation(endType);
        if(endType != EndType.SceneEnd || DialogView.DeactivateOnEndSceneHide)
        {
            HideViewInstant();
        }
        if (endType != EndType.None && isBattle)
        {
            if(AllyBattleBoxManager.instance != null && AllyBattleBoxManager.instance.ShowBattleAlly() != null)
            {
                yield return new WaitForSeconds(0.5f);
            }
            if (!Auto)
            {
                BattleManager.instance.PH.Unpause(PauseSources.Dialog);
            }
        }
        ReadyToContinue = true;
        PH.Pause(PauseSources.Self);
        onComplete?.Invoke();
        OnHideComplete?.Invoke();
        OnHideComplete = null;
    }

    private void HideViewInstant()
    {
        DialogView.gameObject.SetActive(false);
    }

    private bool ShouldHideAllyBox => isBattle 
                                   && lastView == null 
                                   && DialogView != GetView<DialogViewBubble>() 
                                   && AllyBattleBoxManager.instance != null 
                                   && AllyBattleBoxManager.instance.HasActiveCharacter;

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
        else if(ShouldHideAllyBox)
        {
            return StartCoroutine(HideAllyBoxCr());
        }
        return null;
    }

    private IEnumerator HideAllyBoxCr()
    {
        if(AllyBattleBoxManager.instance.HideCharacter() != null)
        {
            yield return new WaitForSeconds(0.2f);
        }
    }

    // Hide all views except for current.
    private IEnumerator ChangeViews(System.Action callback)
    {
        ReadyToContinue = false;
        bool hasLastView = lastView != null;
        if (hasLastView && !lastView.IsHidden)
        {
            yield return lastView.PlayExitAnimation(EndType.None);
            lastView.gameObject.SetActive(false);
        }
        yield return ShowView(!hasLastView, callback); // callback will get called at the end of here
    }

    public void OnSkip()
    {
        onSkip?.Invoke();
    }
}

