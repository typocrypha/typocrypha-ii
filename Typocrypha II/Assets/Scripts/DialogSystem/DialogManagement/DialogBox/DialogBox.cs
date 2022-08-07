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
    const float defaultScrollDelay = 0.001f; // Default text scrolling speed.
    const int defaultScrollBatch = 2; // Default number of characters displayed each scroll.
    const int defaultSpeechInterval = 4; // Default number of characters before speech sfx plays
    const float textPad = 16f; // Padding between text rect and dialog box rect.
    #endregion

    float scrollDelay; // Delay in showing characters for text scroll.
    public float ScrollDelay
    {
        get => scrollDelay * PlayerDataManager.instance.Get<float>(PlayerDataManager.textDelayScale);
        set => scrollDelay = value;
    }
    int speechInterval; // Number of character scrolls before speech sfx plays

    public TextMeshProUGUI dialogText; // Text display component
    public AudioSource[] voiceAS; // AudioSources for playing speech sfx
    public bool resizeTextBox = true; // Should dialog box resize itself?
    [SerializeField] private RectTransform textHolder;
    [SerializeField] FXText.TMProColor hideText; // Allows for hiding parts of text (for scrolling)
    DialogItem dialogItem; // Dialog line data
    Coroutine scrollCR; // Coroutine that scrolls the text

    /// <summary>
    /// Returns whether text is done scrolling or not.
    /// </summary>
    public bool IsDone
    {
        get => scrollCR == null;
    }

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
        speechInterval = defaultSpeechInterval;
        // Remove old text
        dialogText.text = "";
        // Remove old text effects.
        var fxTexts = gameObject.GetComponents<FXText.TMProEffect>();
        foreach (var fxText in fxTexts)
        {
            if(fxText != hideText)
            {
                Destroy(fxText);
            }
        }
        // Hide all text.
        hideText.color = Color.clear;
        hideText.ind[0] = 0;
        hideText.ind[1] = dialogItem.text.Length;
    }

    /// <summary>
    /// Dumps all remaining text.
    /// </summary>
    public void DumpText()
    {
        // End text scroll and display all text.
        if (!IsDone)
        {
            StopCoroutine(scrollCR);
        }
		scrollCR = null;
        hideText.ind[0] = dialogItem.text.Length;
        DialogManager.instance.onSkip.Invoke();
    }

    /// <summary>
    /// Set dialog box's height based on amount of text.
    /// </summary>
    /// <param name="add">Add on size rather than reset size.</param>
    public void SetBoxHeight(bool add = false)
    {
        if (textHolder != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(textHolder);
        }
        RectTransform rectTr = GetComponent<RectTransform>();
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
        //yield return null;
        int pos = 0;
        while (pos < dialogItem.text.Length)
        {
            yield return StartCoroutine(CheckEvents (pos));
            yield return new WaitWhile(() => ph.Pause); // Wait on pause.
            if (pos % speechInterval == 0)
                foreach(var v in voiceAS) if (v != null && v.clip != null) v.Play();
            pos+=defaultScrollBatch; // Advance text position.
            hideText.ind[0] = pos;
            if (ScrollDelay > 0f)
            {
                yield return new WaitForSeconds(ScrollDelay);
            }
            else // If scale is at 0, skip to next dialog event
            {
                if (dialogItem.TextEventList.Count > 0)
                    pos = dialogItem.TextEventList[0].pos;
                hideText.ind[0] = pos;
            }
        }
        yield return StartCoroutine(CheckEvents (dialogItem.text.Length)); // Play events at end of text.
		scrollCR = null;
	}

	// Checks for and plays text events
	IEnumerator CheckEvents(int startPos)
    {
        while (dialogItem.TextEventList.Count > 0 && dialogItem.TextEventList[0].pos <= startPos)
        {
            TextEvent te = dialogItem.TextEventList[0];
            dialogItem.TextEventList.RemoveAt(0);
            TextEvents.instance.PlayEvent(te.evt, te.opt);
            yield return new WaitWhile(() => ph.Pause); // Wait on pause.
        }
        yield return null;
	}
}
