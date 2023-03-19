using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

/// <summary>
/// Processes dialogue from graph into dialog item and dialog box.
/// </summary>
public class DialogParser : MonoBehaviour
{
    public static DialogParser instance = null;

	public static readonly char[] optDelim = new char[1] { ',' }; // Option delimiter
    public static readonly char[] escapeChar = new char[1] { '\\' }; // Escape character
    private static readonly char[] FXTextDelim = new char[2] { '^', '|' }; // FXText delimiters
    private static readonly char[] textEventDelim = new char[2] { '[' , ']' }; // TextEvent delimiters
    private static readonly Dictionary<string, Color32> colorMap = new Dictionary<string, Color32> {
        {"ui-terms", new Color32(5, 171, 255, 255) },
        {"evil-eye", new Color32(255, 0, 66, 255) },
        {"whisper", new Color32(200, 200, 200, 220) },
        {"highlight", new Color32(255, 132, 12, 255) },
        //{ "spell",      "#ff6eff" },
        //{ "enemy-talk", "#974dfe" },
        //{ "enemy-name", "#16e00c" },
        //{ "tips",       "#ffdb16" },
        //{ "highlight",  "#ff840c" },
        //{ "mc",         "#d043e2" },
        //{ "illyia",     "#c70126" },
        //{ "dahlia",     "#8097e0" },
        //{ "doppel",     "#E0015A" }
    };

    Dictionary<string, System.Type> FXTextMap; // Reference FXText effects by name
    Stack<FXText.TMProEffect> FXTextStack; // Stack for managing nested effects

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

		FXTextMap = new Dictionary<string, System.Type> ()
        {
            {"color", typeof(FXText.TMProColor) },
			//{"scramble", typeof(FXText.Scramble)},
			//{"wavy", typeof(FXText.Wavy)},
            {"shake", typeof(FXText.TMProShake)},
            //{"rainbow", typeof(FXText.Rainbow) },
            //{"tips", typeof(FXText.TIPS) }
        };
		FXTextStack = new Stack<FXText.TMProEffect> ();
	}

    public void Parse(DialogItem dialogItem, DialogBox dialogBox, bool createEvents = true)
    {
        Parse(dialogItem, dialogBox.gameObject, dialogBox.dialogText, createEvents);
    }

	/// <summary>
    /// Parses dialog in dialog item.
    /// Parses tags and adds FXText components to dialog box.
    /// </summary>
    /// <param name="dialogItem">Dialog item to modify and parse.</param>
    /// <param name="dialogBox">Dialog box that will hold dialog.</param>
	public void Parse(DialogItem dialogItem, GameObject fxContainer, TextMeshProUGUI textUI, bool createEvents = true)
    {
        string text = dialogItem.text;
        var parsed = new StringBuilder(text.Length);
        dialogItem.Clear();
		bool tag = false; // Are we parsing a tag?
		int i = 0;
		for (; i < text.Length; ++i)
        {
			char c = text [i];
			if (c == FXTextDelim[0]) // FXTextEffect start tag
            { 
				tag = !tag;
				if (tag) ParseEffectStart (i + 1, text, parsed, fxContainer, textUI);
			}
            else if (c == FXTextDelim[1]) // FXTextEffect end tag
            { 
				tag = !tag;
				if (tag) ParseEffectEnd (i + 1, text, parsed, dialogItem);
			}
            else if (c == textEventDelim[0] || c == textEventDelim[1]) // Text Event
            { 
				i = ParseTextEvent (i, text, parsed, dialogItem, createEvents);
			}
            else if (!tag)
            {
                if(c == escapeChar[0])
                {
                    parsed.Append(text[++i]);
                }
				else if(c == '’')
                {
					parsed.Append ("'");
				}
				else
                {
					parsed.Append (c);
				}
			}
		}
        dialogItem.text = parsed.ToString();
	}

    /// <summary>
    /// Parses an effect's starting tag, and adds it to the stack;
    /// </summary>
    /// <param name="startPos">Starting index of tag.</param>
    /// <param name="text">Total raw text of dialog.</param>
    /// <param name="parsed">Currently parsed dialog.</param>
    /// <param name="dialogBox">Dialogbox component reference.</param>
    void ParseEffectStart(int startPos, string text, StringBuilder parsed, GameObject fxContainer, TextMeshProUGUI textUI)
    {
        int endPos = text.IndexOf (FXTextDelim[0], startPos) - 1;
        var args = text.Substring(startPos, endPos - startPos + 1).Split(optDelim);
        var fx = fxContainer.AddComponent(FXTextMap[args[0]]) as FXText.TMProEffect;
        fx.text = textUI; // Set text component reference
        fx.ind = new List<int> {parsed.Length, -1 }; // Set start position: End position set by ParseEffectEnd
        fx.Priority = -10; // Set to low priority
        FXTextStack.Push(fx); // Add to stack
        // Hardcoded check for color effect (extra parameter)
        if (fx is FXText.TMProColor tmProColor)
        {
            tmProColor.color = colorMap[args[1]]; // Set color
        }
    }

	// Parses an effect's ending tag, and matches with top of effect stack
	void ParseEffectEnd(int startPos, string text, StringBuilder parsed, DialogItem dialogItem) 
    {
		int endPos = text.IndexOf (FXTextDelim[1], startPos) - 1;
		string fxName = text.Substring (startPos, endPos - startPos + 1);
        // Hardcoding for color
        var top = FXTextStack.Pop();
        if (FXTextMap[fxName] != top.GetType())
        {
            throw new System.Exception("Mismatched FXTextEffect tags:" + fxName);
        }
        top.ind[1] = parsed.Length;
        dialogItem.FXTextList.Add(top);
    }

	// Parses a Text Event
	int ParseTextEvent(int startPos, string text, StringBuilder parsed, DialogItem dialogItem, bool createEvents)
    {
		int endPos = text.IndexOf (textEventDelim[1], startPos);
        if (createEvents)
        {
            var opt = text.Substring(startPos + 1, endPos - startPos - 1).Split(optDelim, escapeChar);
            string evt = opt[0];
            opt = opt.Skip(1).ToArray();
            dialogItem.TextEventList.Add(new TextEvent(evt, opt, parsed.Length));
        }
		return endPos;
	}

    /// <summary>
    /// Substiutes macros in (curly braces).
    /// </summary>
    /// <param name="text">Text to substitute macros into.</param>
    /// <returns>Text with substitutions.</returns>


    /// <summary>
    /// Removes all tags from dialog text.
    /// </summary>
    /// <param name="text">Dialog text with tags.</param>
    /// <returns>Dialog text w/o tags.</returns>
    public string RemoveTags(string text)
    {
        var parsed = new StringBuilder(text.Length);
        int cnt = 0;
        for(int i = 0; i < text.Length;)
        {
            var c = text[i];
            if (c == FXTextDelim[0])
                i = text.IndexOf(FXTextDelim[0], i + 1, escapeChar) + 1;
            else if (c == FXTextDelim[1])
                i = text.IndexOf(FXTextDelim[1], i + 1, escapeChar) + 1;
            else if (c == textEventDelim[0])
                i = text.IndexOf(textEventDelim[1], i + 1, escapeChar) + 1;
            else if (c == TextMacros.macroDelim[0])
                i = text.IndexOf(TextMacros.macroDelim[1], i + 1, escapeChar) + 1;
            else if (escapeChar.Contains(c))
                i++;
            else
            {
                parsed.Append(c);
                i++;
            }
                
            if (cnt++ > 10000) break; // Infinite loop check
        }
        return parsed.ToString();
    }
}
