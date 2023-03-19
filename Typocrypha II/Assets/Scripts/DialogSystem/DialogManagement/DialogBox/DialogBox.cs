using System.Collections;
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
    float ScrollDelay { get; set; }
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
    const float defaultScrollDelay = 0.021f; // Default text scrolling speed.
    const int defaultTextDisplayInterval = 1; // Default number of characters displayed each scroll.
    const int defaultSpeechInterval = 4; // Default number of characters before speech sfx plays
    const bool defaultPlaySpeechOnSpaces = true;
    const float autoContinueDelay = 0.1f;
    const float textPad = 16f; // Padding between text rect and dialog box rect.
    #endregion

    float scrollDelay = defaultScrollDelay; // Delay in showing characters for text scroll.
    public float ScrollDelay
    {
        get => scrollDelay * PlayerDataManager.instance.Get<float>(PlayerDataManager.textDelayScale);
        set => scrollDelay = value;
    }
    public int SpeechInterval { get; set; } = defaultSpeechInterval; // Number of character scrolls before speech sfx plays
    public bool PlaySpeechOnSpaces { get; set; } = defaultPlaySpeechOnSpaces;

    public TextMeshProUGUI dialogText; // Text display component
    public AudioSource[] voiceAS; // AudioSources for playing speech sfx
    public bool resizeTextBox = true; // Should dialog box resize itself?
    [SerializeField] private RectTransform textHolder;
    [SerializeField] private RectTransform continueIndicator = null;
    [SerializeField] FXText.TMProColor hideText; // Allows for hiding parts of text (for scrolling)
    DialogItem dialogItem; // Dialog line data
    Coroutine scrollCR; // Coroutine that scrolls the text
    private bool started = false;

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
        voiceAS = GetComponents<AudioSource>();
    }

    public void SetupDialogBox(DialogItem dialogItem)
    {
        // Get dialog.
        this.dialogItem = dialogItem;
        ResetDialogBox();
        string rtext = DialogParser.instance.SubstituteMacros(dialogItem.text); // Parse macros
        dialogItem.text = Regex.Replace(rtext, @"<.*?>", ""); // Remove rich text tags
        DialogParser.instance.Parse(dialogItem, this); // Parse w/o rich text tags
        dialogText.text = DialogParser.instance.RemoveTags(rtext); // Set dialog text (doesn't remove rich text tags
        hideText.UpdateAllEffects();
        // Set box size based on text.
        if (resizeTextBox) SetBoxHeight();
        // Set voice sfx.
        if (dialogItem.voice != null)
        {
            if (dialogItem.voice.Count == 0)
            {
                for (int i = 0; i < voiceAS.Length; i++)
                    voiceAS[i].clip = null;
            }
            else
            {
                for (int i = 0; i < dialogItem.voice.Count; i++)
                    voiceAS[i].clip = dialogItem.voice[i];
            }
        }
        started = false;
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
        ScrollDelay = defaultScrollDelay;
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
        DialogManager.instance.onSkip.Invoke();
    }

    /// <summary>
    /// Set dialog box's height based on amount of text.
    /// </summary>
    /// <param name="add">Add on size rather than reset size.</param>
    /// <param name="hasContinueIndicator">Whether the dialog box has a continue indicator.<param>
    public void SetBoxHeight(bool add = false, bool hasContinueIndicator = true)
    {
        if (textHolder != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(textHolder);
        }
        RectTransform rectTr = GetComponent<RectTransform>();
        
        if (hasContinueIndicator && continueIndicator != null &&
            dialogText.preferredWidth % dialogText.rectTransform.sizeDelta.x > continueIndicator.localPosition.x - 20f)
        {
            dialogText.text += "\n\n";
            LayoutRebuilder.ForceRebuildLayoutImmediate(textHolder);
        }
        if (rectTr != null && dialogText != null)
        {
            rectTr.sizeDelta = add
                ? new Vector2(rectTr.sizeDelta.x, rectTr.sizeDelta.y + dialogText.preferredHeight + textPad)
                : new Vector2(rectTr.sizeDelta.x, dialogText.preferredHeight + textPad);
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
        for(int pos = 0; pos < dialogItem.text.Length; ++pos)
        {
            // Check text events at every position regardless of batch size
            if(HasTextEvents() && dialogItem.TextEventList[0].pos <= pos)
            {
                yield return StartCoroutine(CheckEvents(pos));
                if (this.IsPaused())
                {
                    yield return new WaitWhile(this.IsPaused); // Wait on pause.
                }
            }
            // Play scroll blips
            if (pos % SpeechInterval == 0 && (PlaySpeechOnSpaces || !char.IsWhiteSpace(dialogItem.text[pos])))
            {
                foreach (var v in voiceAS)
                {
                    if (v != null && v.clip != null) 
                        v.Play();
                }
            }
            if(pos % defaultTextDisplayInterval == 0)
            {
                hideText.ind[0] = pos + defaultTextDisplayInterval;
            }
            // Apply scroll delay if necessary
            if (ScrollDelay > 0f)
            {
                yield return new WaitForSeconds(ScrollDelay);
            }
            else // If scale is at 0, skip to next dialog event
            {
                DumpText();
                yield break;
            }
        }
        hideText.ind[0] = dialogItem.text.Length;
        hideText.done = true;
        if (HasTextEvents())
        {
            yield return StartCoroutine(CheckEvents(dialogItem.text.Length)); // Play events at end of text.
        }
        if (ShouldAutoContiune())
        {
            yield return new WaitForSeconds(autoContinueDelay);
            DialogManager.instance.NextDialog();
        }
        scrollCR = null;
    }
    private bool HasTextEvents()
    {
        return dialogItem.TextEventList.Count > 0;
    }

    private bool ShouldAutoContiune()
    {
        return dialogItem.text.Length > 0 && dialogItem.text[dialogItem.text.Length - 1] == '-';
    }

	// Checks for and plays text events
	IEnumerator CheckEvents(int startPos)
    {
        while (HasTextEvents() && dialogItem.TextEventList[0].pos <= startPos)
        {
            TextEvent te = dialogItem.TextEventList[0];
            dialogItem.TextEventList.RemoveAt(0);
            var textEventRoutine = TextEvents.instance.PlayEvent(te.evt, te.opt, this);
            if(textEventRoutine != null)
            {
                yield return textEventRoutine;
            }
        }
	}
}
