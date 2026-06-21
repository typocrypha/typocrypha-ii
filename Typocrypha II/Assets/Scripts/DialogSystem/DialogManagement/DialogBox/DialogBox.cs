using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

/// <summary>
/// A single dialog box.
/// </summary>
public class DialogBox : MonoBehaviour, IPausable
{
    #region IPausable
    /// <summary>
    /// Pauses text scroll.
    /// </summary>
    PauseHandle ph;
    public PauseHandle PH { get => ph; }

    public void OnPause(bool b)
    {
    }

    private WaitWhile pauseYielder;
    #endregion

    #region Constants
    const int defaultTextDisplayInterval = 1; // Default number of characters displayed each scroll.
    const int defaultSpeechInterval = 4; // Default number of characters before speech sfx plays
    const bool defaultPlaySpeechOnSpaces = true;
    public const float defaultDashContinueDelay = 0.1f;
    const float defaultAutoContinueDelay = 0.5f;
    #endregion

    public bool Scroll { get; set; }
    public int SpeechInterval { get; set; } = defaultSpeechInterval; // Number of character scrolls before speech sfx plays
    public bool PlaySpeechOnSpaces { get; set; } = defaultPlaySpeechOnSpaces;

    public CanvasGroup CanvasGroup => canvasGroup;
    public DialogContinueIndicator ContinueIndicator
    {
        get => continueIndicator;
        set
        {
            if (continueIndicator != null)
            {
                continueIndicator.PH.FreeFromParent();
            }

            continueIndicator = value;

            if (continueIndicator != null && ph != null)
            {
                continueIndicator.PH.SetParent(ph);
                continueIndicator.PH.PauseIfParentPaused();
            }
        }
    }

    public TextMeshProUGUI DialogText => dialogText;
    [SerializeField] private TextMeshProUGUI dialogText; // Text display component
    [SerializeField] private bool resizeTextBox = true; // Should dialog box resize itself?
    [SerializeField] private bool shrinkToFit = false;
    [SerializeField] private bool resolveContinueIndicatorConflicts = false;
    [SerializeField] private float textPad = 16f; // Padding between text rect and dialog box rect.
    [SerializeField] private RectTransform textHolder;
    [SerializeField] private DialogContinueIndicator continueIndicator;
    [SerializeField] private CanvasGroup canvasGroup = null;
    [SerializeField] FXText.TMProColor hideText; // Allows for hiding parts of text (for scrolling)
    [SerializeField] private bool scroll = true;

    public IReadOnlyList<string> TIPsEntries
    {
        get
        {
            if (dialogItem == null || dialogItem.tipsEntries == null)
                return System.Array.Empty<string>();
            return dialogItem.tipsEntries;
        }
    }
    private DialogItem dialogItem; // Dialog line data
    private Coroutine scrollCR; // Coroutine that scrolls the text
    private AudioClip[] textBlips = new AudioClip[2];
    private bool started = false;
    private float defaultWidth;
    private readonly List<FXText.TMProEffect> nonScrollEffects = new List<FXText.TMProEffect>();

    /// <summary>
    /// Returns whether text is done scrolling or not.
    /// </summary>
    public bool IsDone
    {
        get => started && scrollCR == null;
    }

    public string ID => boxID;
    [SerializeField] private string boxID = "dialogBox";

    void Awake()
    {
        ph = new PauseHandle(OnPause);
        ph.SetParent(DialogManager.instance);
        ph.PauseIfParentPaused();
        if (continueIndicator != null)
        {
            continueIndicator.PH.SetParent(ph);
            continueIndicator.PH.PauseIfParentPaused();
        }
        if (textHolder != null)
        {
            defaultWidth = textHolder.sizeDelta.x;
        }
    }

    public void SetupDialogBox(DialogItem dialogItem)
    {
        // Get dialog.
        this.dialogItem = dialogItem;
        ResetDialogBox();
        // Parse dialog and set text
        DialogParser.instance.Parse(dialogItem, dialogText, gameObject, nonScrollEffects);
        dialogText.text = dialogItem.text;
        // Update all effects manually
        hideText.UpdateAllEffects();
        // Set box size based on text.
        if (resizeTextBox) SetBoxHeight();
        if (shrinkToFit)
        {
            var preferredWidth = Mathf.Min(defaultWidth, dialogText.preferredWidth + 50);
            textHolder.sizeDelta = new Vector2(preferredWidth, textHolder.sizeDelta.y);
        }
        // Set voice sfx.
        if (dialogItem.voice == null || dialogItem.voice.Count == 0)
        {
            for (int i = 0; i < textBlips.Length; i++)
            {
                textBlips[i] = null;
            }
        }
        else
        {
            for (int i = 0; i < textBlips.Length; i++)
            {
                if(i < dialogItem.voice.Count)
                {
                    textBlips[i] = dialogItem.voice[i];
                }
                else
                {
                    textBlips[i] = null;
                }
            }
        }
    }

    public void StartDialogScroll()
    {
        scrollCR = StartCoroutine(TextScrollCR());
    }

    /// <summary>
    /// Initializes dialogue box (parses tags) and starts text scroll.
    /// </summary>
    /// <param name="dialogItem">Dialog line data to display.</param>
    public void SetupAndStartDialogBox(DialogItem dialogItem)
    {
        SetupDialogBox(dialogItem);
        StartDialogScroll();
    }

    /// <summary>
    /// Reset dialog box to default state.
    /// </summary>
    public void ResetDialogBox()
    {
        // Reset parameters
        Scroll = scroll;
        SpeechInterval = defaultSpeechInterval;
        PlaySpeechOnSpaces = defaultPlaySpeechOnSpaces;
        // Remove old text
        dialogText.text = "";
        // Remove old text effects.
        FXText.TMProEffect.Cleanup(nonScrollEffects);
        // Hide all text.
        hideText.color = Color.clear;
        hideText.ind[0] = 0;
        hideText.ind[1] = dialogItem.text.Length;
        hideText.done = false;
        // Reset private vars
        started = false;
        // Reset width
        textHolder.sizeDelta = new Vector2(defaultWidth, textHolder.sizeDelta.y);
    }

    /// <summary>
    /// Dumps all remaining text.
    /// </summary>
    public void DumpText()
    {
        if (!started)
        {
            return;
        }
        // End text scroll and display all text.
        StopAllCoroutines();
        scrollCR = null;
        hideText.ind[0] = dialogItem.text.Length;
        hideText.done = true;
        if(ContinueIndicator != null)
        {
            ContinueIndicator.Activate();
        }
        DialogManager.instance.OnSkip();
    }

    /// <summary>
    /// Set dialog box's height based on amount of text.
    /// </summary>
    /// <param name="hasContinueIndicator">Whether the dialog box has a continue indicator.<param>
    public void SetBoxHeight()
    {
        if (textHolder == null || dialogText == null)
        {
            return;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(dialogText.rectTransform);
        float preferredHeight = dialogText.preferredHeight - 5;
        float lineWidth = textHolder.sizeDelta.x - 20;
        if (resolveContinueIndicatorConflicts && dialogText.preferredWidth % lineWidth >= lineWidth - 40)
        {
            preferredHeight += 30.45f;
        }
        textHolder.sizeDelta = new Vector2(textHolder.sizeDelta.x, preferredHeight);
        RectTransform rectTr = GetComponent<RectTransform>();
        if (rectTr != null)
        {
            rectTr.sizeDelta = new Vector2(rectTr.sizeDelta.x, preferredHeight + textPad);
        }
    }

    /// <summary>
    /// Get dialog box's height.
    /// </summary>
    /// <returns></returns>
    public float GetBoxHeight()
    {
        return GetComponent<RectTransform>().sizeDelta.y;
    }

    // Scrolls text character by character
    protected IEnumerator TextScrollCR()
    {
        started = true;
        int speechCounter = 0;
        bool resetTextBlips = false;
        if (this.IsPaused())
        {
            yield return Yielders.Paused(this, ref pauseYielder); // Wait on pause.
        }
        for (int pos = 0; pos < dialogItem.text.Length; ++pos)
        {
            if (this.IsPaused())
            {
                yield return Yielders.Paused(this, ref pauseYielder); // Wait on pause.
            }
            // Check text events at every position regardless of batch size
            while (dialogItem.TextEventList.Count > 0 && dialogItem.TextEventList[0].pos <= pos)
            {
                var textEventRoutine = ProcessTextEvent(ref resetTextBlips);
                if (textEventRoutine != null)
                {
                    yield return textEventRoutine;
                }
                if (this.IsPaused())
                {
                    yield return Yielders.Paused(this, ref pauseYielder); // Wait on pause.
                }
            }
            if (resetTextBlips)
            {
                speechCounter = 0;
                resetTextBlips = false;
            }
            // Play scroll blips
            if (Scroll && speechCounter % SpeechInterval == 0 && (PlaySpeechOnSpaces || !char.IsWhiteSpace(dialogItem.text[pos])))
            {
                for (int i = 0; i < textBlips.Length; i++)
                {
                    if(textBlips[i] != null)
                    {
                        AudioManager.instance.PlayTextScrollSfx(textBlips[i]);
                    }
                }
            }
            if(pos % defaultTextDisplayInterval == 0)
            {
                hideText.ind[0] = pos + defaultTextDisplayInterval;
            }
            // Apply scroll delay if necessary
            if (Scroll)
            {
                yield return Yielders.FixedUpdate;
            }
            else
            {
                break;
            }
            ++speechCounter;
        }
        hideText.ind[0] = dialogItem.text.Length;
        hideText.done = true;
        if (this.IsPaused())
        {
            yield return Yielders.Paused(this, ref pauseYielder); // Wait on pause.
        }
        // Check text events at every position regardless of batch size
        while (dialogItem.TextEventList.Count > 0)
        {
            var textEventRoutine = ProcessTextEvent(ref resetTextBlips);
            if (textEventRoutine != null)
            {
                yield return textEventRoutine;
            }
            if (this.IsPaused())
            {
                yield return Yielders.Paused(this, ref pauseYielder); // Wait on pause.
            }
        }
        if (ShouldAutoContinue(out float autoDelay))
        {
            // delay between dialogs
            yield return new WaitForSeconds(autoDelay);

            if (this.IsPaused())
            {
                yield return Yielders.Paused(this, ref pauseYielder); // Wait on pause.
            }

            DialogManager.instance.NextDialog(true);
        }
        else if (ContinueIndicator != null)
        {
            ContinueIndicator.Activate();
        }
        scrollCR = null;
    }

    private bool ShouldAutoContinue(out float delay)
    {
        // skip dialog quickly (for debugging purposes)
        if (DialogManager.instance.Auto)
        {
            delay = 0;
            return true;
        }

        // autocontinue if setting is enabled
        if (Settings.AutoContinue)
        {
            delay = defaultAutoContinueDelay;

            // provide time to read instant text
            delay += Scroll ? 0 : dialogItem.text.Length * Settings.TextScrollDelay;

            // scale to user preference
            delay /= Settings.TextScrollSpeed;
            return true;
        }

        // autocontinue dialog even if setting is disabled
        if (dialogItem.text.EndsWith("-"))
        {
            delay = defaultDashContinueDelay;
            return true;
        }
        
        // no autocontinue
        delay = 0;
        return false;
    }

    private Coroutine ProcessTextEvent(ref bool resetTextBlips)
    {
        var textEvent = dialogItem.TextEventList[0];
        dialogItem.TextEventList.RemoveAt(0);
        // Reset text blips if needed
        if (textEvent.evt == TextEvents.pauseEvent)
        {
            resetTextBlips = true;
        }
        return TextEvents.instance.PlayEvent(textEvent.evt, textEvent.opt, this);
    }

    public const float fadeTime = 0.75f;

    public IEnumerator FadeText()
    {
        bool hasColorEffect = false;
        foreach (var effect in nonScrollEffects)
        {
            if (effect is FXText.TMProColor colorFx)
            {
                colorFx.done = false;
                colorFx.UpdateAllEffects();
                Color initialColor = colorFx.color;
                float time = 0;
                void Lerp(float t)
                {
                    time = t;
                    colorFx.color = colorFx.color.WithAlpha(1 - t);
                }
                DOTween.To(() => time, Lerp, 1, fadeTime).OnComplete(() => colorFx.done = true);
                hasColorEffect = true;
            }
        }
        if (hasColorEffect)
        {
            yield return new WaitForSeconds(fadeTime);
        }
        else
        {
            dialogText.alpha = 1;
            yield return dialogText.DOFade(0, fadeTime).WaitForCompletion();
        }
    }
}
