using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A single dialog box.
/// </summary>
public class DialogBox : MonoBehaviour
{
    public float scrollDelay = 0.1f; // Delay in showing characters for text scroll
    public float ScrollDelay
    {
        get
        {
            return scrollDelay /* * scrollScale*/; // Scale based on speed changes
        }
    }
    public int speechInterval = 3; // Number of character scrolls before speech sfx plays

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

    /// <summary>
    /// Initializes dialogue box (parses tags) and starts text scroll.
    /// </summary>
    /// <param name="dialogItem">Dialog line data to display.</param>
    public void StartDialogBox(DialogItem dialogItem)
    {
        this.dialogItem = dialogItem;
        DialogParser.instance.Parse(dialogItem, this);
        dialogText.text = dialogItem.text;
        // RESIZE DIALOGBOX
        scrollCR = StartCoroutine(TextScrollCR());
	}

    /// <summary>
    /// Dumps all remaining text.
    /// </summary>
    public void DumpText()
    {
		//TextEvents.main.stopEvents();
		//TextEvents.main.reset ();
		//yield return TextEvents.main.finishUp (d_item.text_events);
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
        hideText.ind[0] = 0;
        hideText.ind[1] = dialogItem.text.Length;
        int pos = 0;
		while (pos < dialogItem.text.Length)
        {
            yield return StartCoroutine(CheckEvents (pos));
            // PAUSE
			if (dialogItem.text[pos] == '<') // Skip Unity richtext tags
            {
				pos = dialogItem.text.IndexOf ('>', pos + 1) + 1;
				if (pos >= dialogItem.text.Length) break;
			}
            if (pos % speechInterval == 0) audioSpeech.Play();
            pos++; // Advance text position
            hideText.ind[0] = pos;
            yield return new WaitForSeconds(ScrollDelay);
		}
		yield return StartCoroutine(CheckEvents (dialogItem.text.Length)); // Play events at end of text
		scrollCR = null;
	}

	// Checks for and plays text events
	IEnumerator CheckEvents(int start_pos)
    {
        //if (start_pos >= d_item.text_events.Length)
        //	yield break;
        //List<TextEvent> evt_list = d_item.text_events [start_pos];
        //if (evt_list != null && evt_list.Count > 0) {
        //	foreach (TextEvent t in evt_list) {
        //		TextEvents.main.evt_queue.Enqueue(TextEvents.main.playEvent (t.evt, t.opt));
        //		if (t.evt == "pause")
        //			yield return new WaitForSeconds (float.Parse(t.opt[0]));
        //	}
        //	d_item.text_events [start_pos] = null;
        //}
        yield return null;
	}
}
