using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// represents a macro substitution function
public delegate string MacroSubDel(string[] opt);

// event class for text macro substitions
public static class TextMacros 
{
	public static readonly char[] macroDelim = new char[2] { '{', '}' }; // Macro delimiters
	// for substituting macros
	private static readonly Dictionary<string, MacroSubDel> macroMap = new Dictionary<string, MacroSubDel>
	{
		{"temp", MacroTemp},
		{"last-cast", MacroLastCast},
		{"time", MacroTime},
		{"c", MacroColor},
		{"tl", MacroTranslate},
		{"translate", MacroTranslate},
		{"languageTL", MacroTranslatedLanguage},
	}; 
	// Translation remap
    private static readonly Dictionary<char, char> translationMap = new Dictionary<char, char> {
		{'a', 'e' }, {'b', 'd' }, {'c', 'k' }, {'d', 'b' }, {'e', 'i' },
		{'f', 'h' }, {'g', 'h' }, {'h', 'f' }, {'i', 'a' }, {'j', 'z' },
		{'k', 'x' }, {'l', 't' }, {'m', 'l' }, {'n', 'v' }, {'o', 'u' },
		{'p', 'g' }, {'q', 'n' }, {'r', 'w' }, {'s', 'y' }, {'t', 'm' },
		{'u', 'o' }, {'v', 'r' }, {'w', 'p' }, {'x', 'c' }, {'y', 's' },
		{'z', 'j' }, {'.', ',' }, {',', '.' }
	};

	public static string SubstituteMacros(string text)
	{
		var builder = new StringBuilder(text.Length * 2);
		for (int i = 0; i < text.Length;)
		{
			if (text[i] == macroDelim[0])
			{
				int startPos = i + 1;
				int endPos = text.IndexOf(macroDelim[1], startPos, DialogParser.escapeChar);
				string[] macro = text.Substring(startPos, endPos - startPos).Split(DialogParser.optDelim, DialogParser.escapeChar);
				string[] opt = macro.Skip(1).Take(macro.Length - 1).ToArray();
				builder.Append(ApplyMacro(macro[0], opt));
				i = endPos + 1;
			}
			else
			{
				builder.Append(text[i++]);
			}
		}
		return builder.ToString();
	}

	public static string ApplyMacro(string macroName, string[] args)
    {
		if(macroMap.TryGetValue(macroName, out var macro))
        {
			return macro(args);
        }
		Debug.LogError($"Invalid text macro: {macroName}. Returning empty string");
		return string.Empty;
    }

    // Substitutes appropriate entry from temporary string-object database
    // input: variable name
    static string MacroTemp(string[] opt)
    {
        Debug.Log(opt[0]);
        return PlayerDataManager.instance.GetObj(opt[0]).ToString();
    }

	// substitutes last cast spell's attributes
	// input: [0]: string, "elem","root","style" : specifies which part of spell to display (or "all" for whole spell)
	static string MacroLastCast(string[] opt) {
        throw new NotImplementedException();
	}

	// substitutes with current time
	// input: NONE
	static string MacroTime(string[] opt) {
		return DateTime.Now.Hour + ":" + DateTime.Now.Minute;
	}

	// substitutes in appropriate color tag (TMProColor)
	// input: [0]: string, color name (must be implemented in Unity rich tags)
	//             if argument is empty, subsitutes the closing tag '|color|'
	static string MacroColor(string[] opt) {
		if (opt.Length != 0 && opt [0] != null && opt[0] != "") {
			return "^color," + opt[0] + "^";
		} else {
			return "|color|";
		}
	}

	static string MacroTranslate(string[] opt) {
        char[] op = opt[0].ToCharArray(); //char array is faster than StringBuilder here because mutations are simple
        for(int i = 0; i < op.Length; ++i) {
            if (translationMap.ContainsKey(op[i]) && char.IsLower(op[i]))
                op[i] = translationMap[op[i]];
            else if (translationMap.ContainsKey(char.ToLower(op[i])))
                op[i] = char.ToUpper(translationMap[char.ToLower(op[i])]);
        }
        return new string (op);
    }

	static string MacroTranslatedLanguage(string[] opt) {
		return "Ihsuik";
	}
}
