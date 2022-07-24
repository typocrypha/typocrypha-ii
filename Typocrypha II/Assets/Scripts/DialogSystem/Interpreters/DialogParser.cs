using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// Processes dialogue from graph into dialog item and dialog box.
/// </summary>
public class DialogParser : MonoBehaviour
{
    public static DialogParser instance = null;

	Dictionary<string, System.Type> FXTextMap; // Reference FXText effects by name
	Stack<MonoBehaviour> FXTextStack; // Stack for managing nested effects
	char[] optDelim = new char[1] { ',' }; // Option delimiter
    char[] FXTextDelim = new char[2] { '^', '|' }; // FXText delimiters
    char[] TextEventDelim = new char[2] { '[' , ']' }; // TextEvent delimiters
    char[] macroDelim = new char[2] { '{', '}' }; // Macro delimiters
    char[] escapeChar = new char[1] { '\\' }; // Escape character

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
            //{"shake", typeof(FXText.Shake)},
            //{"rainbow", typeof(FXText.Rainbow) },
            //{"tips", typeof(FXText.TIPS) }
        };
		FXTextStack = new Stack<MonoBehaviour> ();
	}

	/// <summary>
    /// Parses dialog in dialog item.
    /// Parses tags and adds FXText components to dialog box.
    /// </summary>
    /// <param name="dialogItem">Dialog item to modify and parse.</param>
    /// <param name="dialogBox">Dialog box that will hold dialog.</param>
	public void Parse(DialogItem dialogItem, DialogBox dialogBox)
    {
		StringBuilder parsed = new StringBuilder(); 
        string text = dialogItem.text;
        dialogItem.FXTextList = new List<MonoBehaviour>();
		dialogItem.TextEventList = new List<TextEvent>();
		bool tag = false; // Are we parsing a tag?
		int i = 0;
		for (; i < text.Length; ++i)
        {
			char c = text [i];
			if (c == FXTextDelim[0]) // FXTextEffect start tag
            { 
				tag = !tag;
				if (tag) ParseEffectStart (i + 1, text, parsed, dialogBox);
			}
            else if (c == FXTextDelim[1]) // FXTextEffect end tag
            { 
				tag = !tag;
				if (tag) ParseEffectEnd (i + 1, text, parsed, dialogItem);
			}
            else if (c == TextEventDelim[0] || c == TextEventDelim[1]) // Text Event
            { 
				i = ParseTextEvent (i, text, parsed, dialogItem);
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
    void ParseEffectStart(int startPos, string text, StringBuilder parsed, DialogBox dialogBox)
    {
        // TEMP: hardcoded color map
        var color_map = new Dictionary<string, Color32> {
            {"ui-terms", new Color32(5, 171, 255, 255) },
            //{ "spell",      "#ff6eff" },
            //{ "ui-terms",   "#05abff" },
            //{ "evil-eye",   "#ff0042" },
            //{ "enemy-talk", "#974dfe" },
            //{ "enemy-name", "#16e00c" },
            //{ "tips",       "#ffdb16" },
            //{ "whisper",    "#c8c8c8" },
            //{ "highlight",  "#ff840c" },
            //{ "mc",         "#d043e2" },
            //{ "illyia",     "#c70126" },
            //{ "dahlia",     "#8097e0" },
            //{ "doppel",     "#E0015A" }
            };

        int endPos = text.IndexOf (FXTextDelim[0], startPos) - 1;
        var args = text.Substring(startPos, endPos - startPos + 1).Split(optDelim);
        var fxName = args[0];
        var colorName = args[1];
        // Hardcoding for color (since only wording effect)
        var fx = dialogBox.gameObject.AddComponent(FXTextMap[fxName]) as FXText.TMProColor;
        fx.text = dialogBox.dialogText; // Set text component reference
        fx.ind = new List<int> {startPos - 1, -1 }; // Set start position: End position set by ParseEffectEnd
        fx.color = color_map[colorName]; // Set color
        FXTextStack.Push(fx);
        /*
         * Old FXText
        FXText.FXTextBase fx = dialogBox.dialogText.gameObject.AddComponent(FXTextMap[fxName]) as FXText.FXTextBase;
		fx.ind = new List<int> {parsed.Length, -1};
		FXTextStack.Push(fx);
        */
    }

	// Parses an effect's ending tag, and matches with top of effect stack
	void ParseEffectEnd(int startPos, string text, StringBuilder parsed, DialogItem dialogItem) 
    {
		int endPos = text.IndexOf (FXTextDelim[1], startPos) - 1;
		string fxName = text.Substring (startPos, endPos - startPos + 1);
        // Hardcoding for color
        var top = FXTextStack.Pop() as FXText.TMProColor;
        if (FXTextMap[fxName] != top.GetType())
        {
            throw new System.Exception("Mismatched FXTextEffect tags:" + fxName);
        }
        top.ind[1] = parsed.Length;
        dialogItem.FXTextList.Add(top);
        /*
         * Old FXText
        FXText.FXTextBase top = FXTextStack.Pop ();
        if (FXTextMap [fxName] != top.GetType ())
        {
            throw new System.Exception("Mismatched FXTextEffect tags:" + fxName);
        }
		top.ind [1] = parsed.Length;
		dialogItem.FXTextList.Add (top);
        */
    }

	// Parses a Text Event
	int ParseTextEvent(int startPos, string text, StringBuilder parsed, DialogItem dialogItem)
    {
		int endPos = text.IndexOf (TextEventDelim[1], startPos);
		string evt;
		string[] opt;
		opt = text.Substring (startPos + 1, endPos - startPos - 1).Split (optDelim, escapeChar);
        evt = opt[0];
        opt = opt.Skip<string>(1).ToArray<string>();
		dialogItem.TextEventList.Add(new TextEvent(evt, opt, parsed.Length));
		//Debug.Log ("text_event:" + evt + ":" + opt.Aggregate("", (acc, next) => acc + ":" + next));
		return endPos;
	}

    /// <summary>
    /// Substiutes macros in (curly braces).
    /// </summary>
    /// <param name="text">Text to substitute macros into.</param>
    /// <returns>Text with substitutions.</returns>
    public string SubstituteMacros(string text)
    {
		StringBuilder trueStr = new StringBuilder ();
		for (int i = 0; i < text.Length;)
        {
			if (text [i] == macroDelim[0])
            {
				int startPos = i + 1;
				int endPos = text.IndexOf (macroDelim[1], startPos, escapeChar);
				string[] macro = text.Substring (startPos, endPos - startPos).Split(optDelim, escapeChar);
				//Debug.Log ("  macro:" + macro.Aggregate("", (acc, next) => acc + "," + next));
				string[] opt = macro.Skip (1).Take (macro.Length - 1).ToArray ();
				string sub = TextMacros.instance.macroMap [macro[0]] (opt);
				trueStr.Append (sub);
				i = endPos + 1;
			}
            else
            {
				trueStr.Append (text [i++]);
			}
		}
		return trueStr.ToString ();
	}

    /// <summary>
    /// Removes all tags from dialog text.
    /// </summary>
    /// <param name="text">Dialog text with tags.</param>
    /// <returns>Dialog text w/o tags.</returns>
    public string RemoveTags(string text)
    {
        var parsed = new StringBuilder();
        int cnt = 0;
        for(int i = 0; i < text.Length;)
        {
            var c = text[i];
            if (c == FXTextDelim[0])
                i = text.IndexOf(FXTextDelim[0], i + 1, escapeChar) + 1;
            else if (c == FXTextDelim[1])
                i = text.IndexOf(FXTextDelim[1], i + 1, escapeChar) + 1;
            else if (c == TextEventDelim[0])
                i = text.IndexOf(TextEventDelim[1], i + 1, escapeChar) + 1;
            else if (c == macroDelim[0])
                i = text.IndexOf(macroDelim[1], i + 1, escapeChar) + 1;
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
