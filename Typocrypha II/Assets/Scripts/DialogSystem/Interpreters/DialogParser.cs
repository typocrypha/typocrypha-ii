﻿using System.Collections;
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
	Stack<FXText.FXTextBase> FXTextStack; // Stack for managing nested effects
	char[] optDelim = new char[1] { ',' }; // Option delimiter
    char[] FXTextDelim = new char[2] { '^', '|' }; // FXText delimiters
    char[] TextEventDelim = new char[3] { '[' , ']' , '=' }; // TextEvent delimiters
    char[] macroDelim = new char[2] { '{', '}' } // Macro delimiters
;
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
            {"color", typeof(FXText.Color) },
			{"scramble", typeof(FXText.Scramble)},
			{"wavy", typeof(FXText.Wavy)},
            {"shake", typeof(FXText.Shake)},
            {"rainbow", typeof(FXText.Rainbow) },
            {"tips", typeof(FXText.TIPS) }
        };
		FXTextStack = new Stack<FXText.FXTextBase> ();
	}

	/// <summary>
    /// Parses dialog in dialog item.
    /// Parses tags and adds FXText components to dialog box.
    /// </summary>
    /// <param name="dialogItem">Dialog item to modify and parse.</param>
    /// <param name="dialogBox">Dialog box that will hold dialog.</param>
	public void Parse(DialogItem dialogItem, DialogBox dialogBox)
    {
		StringBuilder parsed = new StringBuilder(); // Processes string
		string text = SubstituteMacros(dialogItem.text); 
		dialogItem.FXTextList = new List<FXText.FXTextBase>();
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
				if(c == '’')
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
    
    // Parses an effect's starting tag, and adds it to the stack;
    void ParseEffectStart(int startPos, string text, StringBuilder parsed, DialogBox dialogBox)
    {
		int endPos = text.IndexOf (FXTextDelim[0], startPos) - 1;
		string fxName = text.Substring (startPos, endPos - startPos + 1);
        FXText.FXTextBase fx = dialogBox.dialogText.gameObject.AddComponent(FXTextMap[fxName]) as FXText.FXTextBase;
		fx.ind = new List<int> {parsed.Length, -1};
		FXTextStack.Push(fx);
	}

	// Parses an effect's ending tag, and matches with top of effect stack
	void ParseEffectEnd(int startPos, string text, StringBuilder parsed, DialogItem dialogItem) 
    {
		int endPos = text.IndexOf (FXTextDelim[1], startPos) - 1;
		string fxName = text.Substring (startPos, endPos - startPos + 1);
        FXText.FXTextBase top = FXTextStack.Pop ();
        if (FXTextMap [fxName] != top.GetType ())
        {
            throw new System.Exception("Mismatched FXTextEffect tags:" + fxName);
        }
		top.ind [1] = parsed.Length;
		dialogItem.FXTextList.Add (top);
	}

	// Parses a Text Event
	int ParseTextEvent(int startPos, string text, StringBuilder parsed, DialogItem dialogItem)
    {
		int endPos = text.IndexOf (TextEventDelim[1], startPos);
		int eqPos = text.IndexOf (TextEventDelim[2], startPos);
		string evt;
		string[] opt;
		if (eqPos == -1 || eqPos > endPos) // If no '='
        { 
			evt = text.Substring (startPos + 1, endPos - startPos - 1);
			opt = new string[0]; 
		}
        else
        {
			evt = text.Substring (startPos + 1, eqPos - startPos - 1);
			opt = text.Substring (eqPos + 1, endPos - eqPos - 1).Split (optDelim);
		}
		dialogItem.TextEventList.Add(new TextEvent(evt, opt, parsed.Length));
		//Debug.Log ("  text_event:" + evt + ":" + opt.Aggregate("", (acc, next) => acc + "," + next));
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
				int endPos = text.IndexOf (macroDelim[1], startPos);
				string[] macro = text.Substring (startPos, endPos - startPos).Split(optDelim);
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
}
