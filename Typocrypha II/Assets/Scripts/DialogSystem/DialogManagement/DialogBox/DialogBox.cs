﻿using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Interface with general dialog box functionality.
/// </summary>
public interface IDialogBox : IPausable
{
    bool Scroll { get; set; }
    bool IsDone { get; }
    void DumpText();
}

/// <summary>
/// A single dialog box.
/// </summary>
public class DialogBox : MonoBehaviour, IDialogBox
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
        set => continueIndicator = value;
    }

    public TextMeshProUGUI dialogText; // Text display component
    public bool resizeTextBox = true; // Should dialog box resize itself?
    [SerializeField] private bool shrinkToFit = false;
    [SerializeField] private bool resolveContinueIndicatorConflicts = false;
    [SerializeField] private float textPad = 16f; // Padding between text rect and dialog box rect.
    [SerializeField] private RectTransform textHolder;
    [SerializeField] private DialogContinueIndicator continueIndicator;
    [SerializeField] private CanvasGroup canvasGroup = null;
    [SerializeField] FXText.TMProColor hideText; // Allows for hiding parts of text (for scrolling)
    [SerializeField] private bool scroll = true;
    DialogItem dialogItem; // Dialog line data
    Coroutine scrollCR; // Coroutine that scrolls the text
    private AudioClip[] textBlips = new AudioClip[2];
    private bool started = false;
    private bool resetTextBlips = false;
    private float defaultWidth;

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
        DialogParser.instance.Parse(dialogItem, this);
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
    /// Initializes dialogue box (parses tags) and starts text scroll.
    /// Uses only text (no character name/speech effect/etc).
    /// </summary>
    /// <param name="dialogText">Dialog text to display.</param>
    public void StartDialogBox(string dialogText)
    {
        DialogItem ditem = new DialogItemAN(dialogText, null);
        SetupAndStartDialogBox(ditem);
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
        FXText.TMProEffect.Cleanup(gameObject, hideText);
        // Hide all text.
        hideText.color = Color.clear;
        hideText.ind[0] = 0;
        hideText.ind[1] = dialogItem.text.Length;
        hideText.done = false;
        // Reset private vars
        resetTextBlips = false;
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
        DialogManager.instance.onSkip.Invoke();
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
        resetTextBlips = false;
        if (this.IsPaused())
        {
            yield return new WaitWhile(this.IsPaused); // Wait on pause.
        }
        for (int pos = 0; pos < dialogItem.text.Length; ++pos)
        {
            if (this.IsPaused())
            {
                yield return new WaitWhile(this.IsPaused); // Wait on pause.
            }
            // Check text events at every position regardless of batch size
            if (HasTextEvents() && dialogItem.TextEventList[0].pos <= pos)
            {
                yield return StartCoroutine(CheckEvents(pos));
                if (this.IsPaused())
                {
                    yield return new WaitWhile(this.IsPaused); // Wait on pause.
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
                yield return new WaitForFixedUpdate();
            }
            else // If scale is at 0, skip to next dialog event
            {
                DumpText();
                yield break;
            }
            ++speechCounter;
        }
        hideText.ind[0] = dialogItem.text.Length;
        hideText.done = true;
        if (this.IsPaused())
        {
            yield return new WaitWhile(this.IsPaused); // Wait on pause.
        }
        if (HasTextEvents())
        {
            yield return StartCoroutine(CheckEvents(dialogItem.text.Length)); // Play events at end of text.
        }
        if (ShouldAutoContinue(out float autoDelay))
        {
            yield return new WaitForSeconds(autoDelay);
            if (this.IsPaused())
            {
                yield return new WaitWhile(this.IsPaused); // Wait on pause.
            }
            DialogManager.instance.NextDialog(true, false);
        }
        else if (ContinueIndicator != null)
        {
            continueIndicator.Activate();
        }
        scrollCR = null;
    }
    private bool HasTextEvents()
    {
        return dialogItem.TextEventList.Count > 0;
    }

    private bool ShouldAutoContinue(out float delay)
    {
        if (Settings.AutoContinue)
        {
            delay = defaultAutoContinueDelay / Settings.TextScrollSpeed;
            return true;
        }

        if (dialogItem.text.Length > 0 && dialogItem.text[dialogItem.text.Length - 1] == '-')
        {
            delay = defaultDashContinueDelay;
            return true;
        }
        
        delay = 0;
        return false;
    }

    // Checks for and plays text events
    IEnumerator CheckEvents(int startPos)
    {
        while (HasTextEvents() && dialogItem.TextEventList[0].pos <= startPos)
        {
            TextEvent te = dialogItem.TextEventList[0];
            dialogItem.TextEventList.RemoveAt(0);
            var textEventRoutine = TextEvents.instance.PlayEvent(te.evt, te.opt, this);
            if(ShouldResetTextBlips(te.evt))
            {
                resetTextBlips = true;
            }
            if(textEventRoutine != null)
            {
                yield return textEventRoutine;
            }
        }
    }

    private static bool ShouldResetTextBlips(string evt)
    {
        return evt == TextEvents.pauseEvent;
    }
}
