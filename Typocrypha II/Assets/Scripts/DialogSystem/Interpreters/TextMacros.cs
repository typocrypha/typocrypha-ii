﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// represents a macro substitution function
public delegate string MacroSubDel(string[] opt);

// event class for text macro substitions
public class TextMacros : MonoBehaviour {
	public static TextMacros instance = null; // global static ref
	public Dictionary<string, MacroSubDel> macroMap; // for substituting macros
	public Dictionary<string, string> color_map; // for color presets (hex representation)
	public Dictionary<string, System.Tuple<string, string>> character_map; // for character dialogue presets
    public Dictionary<char, char> translate_map;

	void Awake () {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
		macroMap = new Dictionary<string, MacroSubDel>
        {
            {"temp", macroTemp},
            {"name", macroNameSub},
			{"NAME", macroNameSub},
			{"pronoun",macroPronoun},
            {"last-cast", macroLastCast},
            {"time", macroTime},
			{"c", macroColor},
			{"t", macroSetTalkSfx},
			{"h", macroHighlightCharacter},
            {"hc", macroHighlightCodec},
            {"speak", macroSpeaker},
            {"tl", macroTranslate},
            {"translate", macroTranslate},
			{"languageTL", macroTranslatedLanguage},
			{"rc", macroRemoveCharacter}
        };
		color_map = new Dictionary<string, string> {
			{ "spell",      "#ff6eff" },
			{ "ui-terms",   "#05abff" },
			{ "evil-eye",   "#ff0042" },
			{ "enemy-talk", "#974dfe" },
			{ "enemy-name", "#16e00c" },
			{ "tips",       "#ffdb16" },
			{ "whisper",    "#c8c8c8" },
			{ "highlight",  "#ff840c" },
			{ "mc",         "#d043e2" },
			{ "illyia",     "#c70126" },
			{ "dahlia",     "#8097e0" },
			{ "doppel",     "#E0015A" }
		};
		character_map = new Dictionary<string, System.Tuple<string, string>> {
			{"dahlia", new System.Tuple<string, string>("dahlia", "vo_dahlia") },
			{"illyia", new System.Tuple<string, string>("illyia", "vo_illyia") },
			{"mackey", new System.Tuple<string, string>("mackey", "vo_mackey") },
			{"mc", new System.Tuple<string, string>("_mc_", "vo_mc")},
			{"doppelganger", new System.Tuple<string, string>("doppelganger", "vo_doppelganger")},
			{"clarke", new System.Tuple<string, string>("clarke", "vo_clarke")},
			{"iris", new System.Tuple<string, string>("iris", "vo_iris")},
			{"cat", new System.Tuple<string, string>("cat", "vo_cat")},
			{"cat_person", new System.Tuple<string, string>("cat_person", "vo_cat_person")},
			{"evil_eye", new System.Tuple<string, string>("evil_eye", "vo_evil_eye")},
			{"marcel", new System.Tuple<string, string>("marcel", "vo_marcel")}
		};
        translate_map = new Dictionary<char, char> {
            {'a', 'e' }, {'b', 'd' }, {'c', 'k' }, {'d', 'b' }, {'e', 'i' },
            {'f', 'h' }, {'g', 'h' }, {'h', 'f' }, {'i', 'a' }, {'j', 'z' },
            {'k', 'x' }, {'l', 't' }, {'m', 'l' }, {'n', 'v' }, {'o', 'u' },
            {'p', 'g' }, {'q', 'n' }, {'r', 'w' }, {'s', 'y' }, {'t', 'm' },
            {'u', 'o' }, {'v', 'r' }, {'w', 'p' }, {'x', 'c' }, {'y', 's' },
            {'z', 'j' }, {'.', ',' }, {',', '.' }
        };
	}

    // Substitutes appropriate entry from temporary string-object database
    // input: variable name
    string macroTemp(string[] opt)
    {
        Debug.Log(opt[0]);
        return PlayerDataManager.instance[opt[0]].ToString();
    }

	// substitutes player's name
	// input: NONE
	string macroNameSub(string[] opt) {
		return "NOT IMPLEMENTED";
        //return PlayerDataManager.main.PlayerName;
    }

	// substitutes in appropriate pronoun term
	// choice is made based on 'PlayerDialogueInfo' field
	// input: [0]: string: appropriate term for FEMININE pronoun
	// input: [1]: string: appropriate term for INCLUSIVE pronoun
	// input: [2]: string: appropriate term for FIRSTNAME pronoun
	//   NOTE: input string is concatenated after player's name
	// input: [3]: string: appropriate term for MASCULINE pronoun
	string macroPronoun(string[] opt) {
        return "NOT IMPLEMENTED";
		//switch (PlayerDataManager.main.player_pronoun) {
		//case Pronoun.FEMININE:  return opt [0];
		//case Pronoun.INCLUSIVE: return opt [1];
		//case Pronoun.FIRSTNAME: return PlayerDataManager.main.PlayerName + opt [2];
		//case Pronoun.MASCULINE: return opt [3];
		//default: return "pronoun";
		//}
	}

	// substitutes last cast spell's attributes
	// input: [0]: string, "elem","root","style" : specifies which part of spell to display (or "all" for whole spell)
	string macroLastCast(string[] opt) {
        throw new System.NotImplementedException();
  //      string ret = string.Empty;
		//switch (opt [0]) {
		//    case "elem":  ret = BattleManagerS.main.field.lastSpell.element.ToUpper(); break;
		//    case "root":  ret =  BattleManagerS.main.field.lastSpell.root.ToUpper(); break;
		//    case "style": ret = BattleManagerS.main.field.lastSpell.style.ToUpper(); break;
  //          case "all": ret = BattleManagerS.main.field.lastSpell.ToString(); break;
  //          default:      return "error: bad spell substitute macro argument";	
		//}
		//return "<color=" + color_map["spell"] + ">" + ret + "</color>";
	}

    // substitutes with current time
    // input: NONE
    string macroTime(string[] opt) {
		return System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute;
	}

	// substitutes in appropriate color tag
	// input: [0]: string, color name (must be implemented in Unity rich tags)
	//             if argument is empty, subsitutes the closing tag '</color>'
	string macroColor(string[] opt) {
		if (opt.Length != 0 && opt [0] != null && opt[0] != "") {
			if (color_map.ContainsKey(opt[0]))
				return "<color=" + color_map[opt[0]] + ">";
			else return "<color=" + opt [0] + ">";
		} else {
			return "</color>";
		}
	}

	// substitues in 'set-talk-sfx' TextEvent
	// input: [0]: string, name of audio file
	string macroSetTalkSfx(string[] opt) {
		return "[set-talk-sfx=vo_" + opt[0] + "]";
	}

	// substitutes in 'highlight-character' TextEvent.
	// input: [0]: string, name of sprite to highlight
	//        [1]: float, amount to highlight (multiplier to tint)
	string macroHighlightCharacter(string[] opt) {
		return "[highlight-character=" + opt[0] + "," + opt[1] + "]";
	}

    // substitutes in 'highlight-codec' TextEvent.
    string macroHighlightCodec(string[] opt)
    {
        return "[sole-highlight-codec]";
    }


    // substitutes combined 'set-talk-sfx' and 'highlight-character'
    // solely highlights given character, and switches to their talk sfx
    // input: [0]: string, name of character (see character map)
    string macroSpeaker(string[] opt) {
		string macro = 
			"[set-talk-sfx=" + character_map[opt[0]].Item2 + "]" +
			"[sole-highlight=" + character_map[opt[0]].Item1 + "]";
		return macro;
	}

    string macroTranslate(string[] opt) {
        char[] op = opt[0].ToCharArray(); //char array is faster than StringBuilder here because mutations are simple
        for(int i = 0; i < op.Length; ++i) {
            if (translate_map.ContainsKey(op[i]) && char.IsLower(op[i]))
                op[i] = translate_map[op[i]];
            else if (translate_map.ContainsKey(char.ToLower(op[i])))
                op[i] = char.ToUpper(translate_map[char.ToLower(op[i])]);
        }
        return new string (op);
    }

	string macroTranslatedLanguage(string[] opt) {
		return "Ihsuik";
	}

	// substitutes 'remove-character'
	// input: [0]: string, name of character sprite (spr_vn_ omitted)
	string macroRemoveCharacter(string[] opt) {
		string macro = 
			"[remove-character=spr_vn_" + opt[0] + "]";
		return macro;
	}
}
