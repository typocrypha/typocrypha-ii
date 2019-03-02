using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    #endregion

    #region Constants
    const float defaultScrollDelay = 0.01f; // Default text scrolling speed.
    const int defaultSpeechInterval = 2; // Default number of text scrolls before speech sfx plays
    const float textPad = 16f; // Padding between text rect and dialog box rect.
    #endregion

    float scrollDelay; // Delay in showing characters for text scroll.
    public float ScrollDelay
    {
        get => scrollDelay * (float)PlayerDataManager.instance[PlayerDataManager.textDelayScale];
        set => scrollDelay = value;
    }
    int speechInterval; // Number of character scrolls before speech sfx plays

    public Text dialogText; // Text display component
    public AudioSource voiceAS; // AudioSource for playing speech sfx

    FXText.Color hideText; // Allows for hiding parts of text (for scrolling)
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
    }

    /// <summary>
    /// Initializes dialogue box (parses tags) and starts text scroll.
    /// </summary>
    /// <param name="dialogItem">Dialog line data to display.</param>
    public void StartDialogBox(DialogItem dialogItem)
    {
        ResetDialogBox();
        // Get dialog.
        this.dialogItem = dialogItem;
        DialogParser.instance.Parse(dialogItem, this);
        dialogText.text = dialogItem.text;
        // Hide all text.
        hideText = dialogText.gameObject.AddComponent<FXText.Color>();
        hideText.ind = new List<int> { 0, 0 };
        hideText.color = Color.clear;
        hideText.ind[0] = 0;
        hideText.ind[1] = dialogItem.text.Length;
        SetBoxHeight();
        // Set voice sfx.
        voiceAS.clip = dialogItem.voice;
        scrollCR = StartCoroutine(TextScrollCR());
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
        var fxTexts = dialogText.GetComponents<FXText.FXTextBase>();
        foreach (var fxText in fxTexts)
        {
            Destroy(fxText);
        }
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
	}

    /// <summary>
    /// Set dialog box's height based on amount of text.
    /// </summary>
    public void SetBoxHeight()
    {
        RectTransform rectTr = GetComponent<RectTransform>();
        rectTr.sizeDelta = new Vector2(rectTr.sizeDelta.x, dialogText.preferredHeight + textPad);
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
        yield return null;
        int pos = 0;
		while (pos < dialogItem.text.Length)
        {
            yield return StartCoroutine(CheckEvents (pos));
            yield return new WaitWhile(() => ph.Pause); // Wait on pause.
			if (dialogItem.text[pos] == '<') // Skip Unity richtext tags.
            {
				pos = dialogItem.text.IndexOf ('>', pos + 1) + 1;
				if (pos >= dialogItem.text.Length) break;
			}
            if (pos % speechInterval == 0) voiceAS.Play();
            pos++; // Advance text position.
            hideText.ind[0] = pos;
            if (ScrollDelay > 0f)
            {
                yield return new WaitForSeconds(ScrollDelay);
            }
            else // If scale is at 0, skip all text.
            {
                hideText.ind[0] = dialogItem.text.Length;
                break;
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
