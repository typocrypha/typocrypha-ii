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
    bool pause;
    public bool Pause
    {
        get
        {
            return pause;
        }
        set
        {
            pause = value;
        }
    }
    #endregion

    #region Constants
    const float defaultScrollDelay = 0.05f; // Default text scrolling speed.
    const int defaultSpeechInterval = 3; // Default number of text scrolls before speech sfx plays
    #endregion

    float scrollDelay; // Delay in showing characters for text scroll.
    public float ScrollDelay
    {
        get
        {
            return scrollDelay /* * scrollScale*/; // Scale based on speed changes.
        }
        set
        {
            scrollDelay = value;
        }
    }
    int speechInterval; // Number of character scrolls before speech sfx plays

    public FXText.Color hideText; // Allows for hiding parts of text (for scrolling)
    public Text dialogText; // Text display component
    public AudioSource audioSpeech; // AudioSource for playing speech sfx

    DialogItem dialogItem; // Dialog line data
    Coroutine scrollCR; // Coroutine that scrolls the text

    /// <summary>
    /// Returns whether text is done scrolling or not.
    /// </summary>
    public bool IsDone
    {
        get
        {
            return scrollCR == null;
        }
    }

    void Awake()
    {
        ScrollDelay = defaultScrollDelay;
        speechInterval = defaultSpeechInterval;
    }

    /// <summary>
    /// Initializes dialogue box (parses tags) and starts text scroll.
    /// </summary>
    /// <param name="dialogItem">Dialog line data to display.</param>
    public void StartDialogBox(DialogItem dialogItem)
    {
        ResetDialogBox();
        this.dialogItem = dialogItem;
        DialogParser.instance.Parse(dialogItem, this);
        dialogText.text = dialogItem.text;
        // RESIZE DIALOGBOX
        scrollCR = StartCoroutine(TextScrollCR());
	}

    /// <summary>
    /// Reset dialog box to default state.
    /// </summary>
    public void ResetDialogBox()
    {
        // Remove old text effects.
        var fxTexts = dialogText.GetComponents<FXText.FXTextBase>();
        foreach (var fxText in fxTexts)
        {
            if (fxText != hideText)
            {
                Destroy(fxText);
            }
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

	// Scrolls text character by character
	IEnumerator TextScrollCR()
    {
        yield return null;
        hideText.ind[0] = 0;
        hideText.ind[1] = dialogItem.text.Length;
        int pos = 0;
		while (pos < dialogItem.text.Length)
        {
            yield return StartCoroutine(CheckEvents (pos));
            yield return new WaitWhile(() => Pause); // Wait on pause.
			if (dialogItem.text[pos] == '<') // Skip Unity richtext tags.
            {
				pos = dialogItem.text.IndexOf ('>', pos + 1) + 1;
				if (pos >= dialogItem.text.Length) break;
			}
            if (pos % speechInterval == 0) audioSpeech.Play();
            pos++; // Advance text position.
            hideText.ind[0] = pos;
            yield return new WaitForSeconds(ScrollDelay);
		}
		yield return StartCoroutine(CheckEvents (dialogItem.text.Length)); // Play events at end of text.
		scrollCR = null;
	}

	// Checks for and plays text events
	IEnumerator CheckEvents(int startPos)
    {
        while(dialogItem.TextEventList.Count > 0 && dialogItem.TextEventList[0].pos == startPos)
        {
            TextEvent te = dialogItem.TextEventList[0];
            dialogItem.TextEventList.RemoveAt(0);
            TextEvents.instance.PlayEvent(te.evt, te.opt);
            yield return new WaitWhile(() => Pause); // Wait on pause.
        }
        yield return null;
	}
}
