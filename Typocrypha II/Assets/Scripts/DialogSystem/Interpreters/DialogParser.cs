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
    public const string colorTIPs = "tipsEntry";
    public static DialogParser instance = null;

	public static readonly char[] optDelim = new char[1] { ',' }; // Option delimiter
    public static readonly char[] escapeChar = new char[1] { '\\' }; // Escape character
    private static readonly char[] fxTextDelim = new char[2] { '^', '|' }; // FXText delimiters
    private static readonly char[] textEventDelim = new char[2] { '[' , ']' }; // TextEvent delimiters
    private static readonly char[] richTextDelim = new char[2] { '<' , '>' }; // Rich text delimiters
    private static readonly Dictionary<string, Color32> colorMap = new Dictionary<string, Color32> {
        {"ui-terms", new Color32(5, 171, 255, 255) },
        {"evil-eye", new Color32(255, 0, 66, 255) },
        {"whisper", new Color32(200, 200, 200, 220) },
        {"highlight", new Color32(255, 132, 12, 255) },
        {colorTIPs, new Color32(255, 132, 12, 255) },
        {"ayin", new Color32(176, 167, 255, 255) },
        {"illyia", new Color32(199, 1, 38, 255) },
        {"dahlia", new Color32(128, 151, 224, 255) },
        {"esaias", new Color32(74, 209, 150, 255) },
        {"adelai", new Color32(255, 176, 197, 255) },
        {"huginn", new Color32(168, 86, 185, 255) },
        {"muninn", new Color32(86, 185, 168, 255) }
        //{ "spell",      "#ff6eff" },
        //{ "enemy-talk", "#974dfe" },
        //{ "enemy-name", "#16e00c" },
        //{ "tips",       "#ffdb16" },
        //{ "highlight",  "#ff840c" },
        //{ "doppel",     "#E0015A" }
    };
    // Reference FXText effects by name
    private readonly Dictionary<string, System.Type> fxTextMap = new Dictionary<string, System.Type>()
    {
        {"color", typeof(FXText.TMProColor) },
		//{"scramble", typeof(FXText.Scramble)},
		//{"wavy", typeof(FXText.Wavy)},
        {"shake", typeof(FXText.TMProShake)},
        //{"rainbow", typeof(FXText.Rainbow) },
    };

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
	}

    public void Parse(DialogItem dialogItem, TextMeshProUGUI text, GameObject fxContainer, List<FXText.TMProEffect> fxList, bool createEvents = true)
    {
        dialogItem.text = Parse(dialogItem.text, fxContainer, text, dialogItem.TextEventList, dialogItem.FXTextList, out dialogItem.tipsEntries, createEvents);
        fxList.AddRange(dialogItem.FXTextList);
    }

    // Parse without text events
    public string Parse(string line, GameObject fxContainer, TextMeshProUGUI textUI, List<FXText.TMProEffect> textEffects, out List<string> tipsEntries)
    {
        return Parse(line, fxContainer, textUI, null, textEffects, out tipsEntries, false);
    }

    // Parses a line of text removing, creating text FX and parsing text events.
    // Text event creation is optional, in which case false should be passed in the create events arg
	public string Parse(string line, GameObject fxContainer, TextMeshProUGUI textUI, List<TextEvent> textEvents, List<FXText.TMProEffect> textEffects, out List<string> tipsEntries, bool createEvents = true)
    {
        // Clear output lists
        textEvents?.Clear();
        textEffects.Clear();
        Stack<FXText.TMProEffect> effectStack = null;
        // Initialize text (substitute macros) Regex.Replace(line, @"<.*?>", ""); // Remove rich text tags
        string text = TextMacros.SubstituteMacros(line, out tipsEntries);
        // Initialize parsing vars
        var parsed = new StringBuilder(text.Length);
		bool tag = false; // Are we parsing a tag?
        int effectIndex = 0;
		for (int i = 0; i < text.Length; ++i)
        {
			char c = text [i];
            if(c == richTextDelim[0])
            {
                i = ParseRichTextTag(i, text, parsed);
            }
			else if (c == fxTextDelim[0]) // FXTextEffect start tag
            { 
				tag = !tag;
                if (tag)
                {
                    ParseEffectStart(i + 1, effectIndex, text, fxContainer, textUI, ref effectStack);
                }
			}
            else if (c == fxTextDelim[1]) // FXTextEffect end tag
            { 
				tag = !tag;
                if (tag && TryParseEffectEnd(i + 1, effectIndex, text, ref effectStack, out var effect))
                {
                    textEffects.Add(effect);
                }
			}
            else if (c == textEventDelim[0] || c == textEventDelim[1]) // Text Event
            { 
				i = ParseTextEvent(i, text, parsed, textEvents, createEvents);
			}
            else if (!tag)
            {
                if(c == escapeChar[0])
                {
                    parsed.Append(text[++i]);
                    effectIndex++;
                }
				else if(c == '’')
                {
					parsed.Append ("'");
				}
				else
                {
					parsed.Append (c);
				}
                effectIndex++;
			}
		}
        return parsed.ToString();
	}

    /// <summary>
    /// Parses an effect's starting tag, and adds it to the stack;
    /// </summary>
    /// <param name="startPos">Starting index of tag.</param>
    /// <param name="text">Total raw text of dialog.</param>
    /// <param name="parsed">Currently parsed dialog.</param>
    /// <param name="dialogBox">Dialogbox component reference.</param>
    private void ParseEffectStart(int startPos, int effectIndex, string text, GameObject fxContainer, TextMeshProUGUI textUI, ref Stack<FXText.TMProEffect> effectStack)
    {
        effectStack = effectStack ?? new Stack<FXText.TMProEffect>();
        int endPos = text.IndexOf (fxTextDelim[0], startPos) - 1;
        var args = text.Substring(startPos, endPos - startPos + 1).Split(optDelim);
        var fx = fxContainer.AddComponent(fxTextMap[args[0]]) as FXText.TMProEffect;
        fx.text = textUI; // Set text component reference
        fx.ind = new List<int> {effectIndex, -1 }; // Set start position: End position set by ParseEffectEnd
        fx.Priority = -10; // Set to low priority
        effectStack.Push(fx); // Add to stack
        // Hardcoded check for color effect (extra parameter)
        if (fx is FXText.TMProColor tmProColor)
        {
            tmProColor.color = colorMap[args[1]]; // Set color
        }
    }

	// Parses an effect's ending tag, and matches with top of effect stack
	private bool TryParseEffectEnd(int startPos, int effectIndex, string text, ref Stack<FXText.TMProEffect> effectStack, out FXText.TMProEffect effect) 
    {
        effectStack = effectStack ?? new Stack<FXText.TMProEffect>();
		int endPos = text.IndexOf (fxTextDelim[1], startPos) - 1;
		string fxName = text.Substring (startPos, endPos - startPos + 1);
        // Hardcoding for color
        if(effectStack.Count <= 0)
        {
            Debug.LogError("Trying to pop empty text effect stack");
            effect = null;
            return false;
        }
        var top = effectStack.Pop();
        if (fxTextMap[fxName] != top.GetType())
        {
            Debug.LogError("Mismatched FXTextEffect tags:" + fxName);
            effect = null;
            return false;
        }
        top.ind[1] = effectIndex;
        effect = top;
        return true;
    }

	// Parses a Text Event
	int ParseTextEvent(int startPos, string text, StringBuilder parsed, List<TextEvent> textEventList, bool createEvents)
    {
		int endPos = text.IndexOf (textEventDelim[1], startPos);
        if (createEvents)
        {
            var opt = text.Substring(startPos + 1, endPos - startPos - 1).Split(optDelim, escapeChar);
            string evt = opt[0];
            opt = opt.Skip(1).ToArray();
            textEventList.Add(new TextEvent(evt, opt, parsed.Length));
        }
		return endPos;
	}

    // Parses a Text Event
    int ParseRichTextTag(int startPos, string text, StringBuilder parsed)
    {
        int endPos = text.IndexOf(richTextDelim[1], startPos);
        parsed.Append(text.Substring(startPos, endPos - startPos + 1));
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
        //var rText = Regex.Replace(dialogItem.text, @"<.*?>", ""); // Remove rich text tags
        var parsed = new StringBuilder(text.Length);
        int cnt = 0;
        for(int i = 0; i < text.Length;)
        {
            var c = text[i];
            if (c == fxTextDelim[0])
                i = text.IndexOf(fxTextDelim[0], i + 1, escapeChar) + 1;
            else if (c == fxTextDelim[1])
                i = text.IndexOf(fxTextDelim[1], i + 1, escapeChar) + 1;
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
