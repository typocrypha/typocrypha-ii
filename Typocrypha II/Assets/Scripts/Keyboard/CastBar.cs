using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using TMPro;

namespace Typocrypha
{
    /// <summary>
    /// Manages Cast Bar interface for battle.
    /// </summary>
    public class CastBar : MonoBehaviour
    {
        public TextMeshPro[] letters; // Array of single letter rects.
        public Transform cursor; // Cursor for keeping track of position.
        public GameObject[] keywords; // Set of all keywords (prefab effects).
        public UnityEvent_string onCast; // Event called when enter is pressed (casting spell).

        protected readonly StringBuilder sb = new StringBuilder(); // String builder for text.
        public string Text
        {
            get => sb.ToString();
        }

        protected int pos = 0; // Cursor position.
        readonly Dictionary<string, GameObject> keywordMap = new Dictionary<string, GameObject>();
        readonly Regex alpha = new Regex("^[A-Za-z]"); // Matches alphabetic strings.
        readonly char[] keywordDelim = new char[1] { ' ' };
        protected virtual char SpaceChar => ' ';

        void Start()
        {
            letters = GetComponentsInChildren<TextMeshPro>();
            Clear();
            foreach (var pf in keywords)
            {
                keywordMap[pf.name] = pf;
            }
        }

        void Update()
        {
            cursor.GetComponent<RectTransform>().offsetMin = // Set cursor position.
                letters[pos].GetComponent<RectTransform>().offsetMin;
        }

        protected virtual void HandleBackSpace()
        {
            letters[--pos].text = "";
            sb.Remove(pos, 1);
        }

        private bool CheckSpecialCharacter(char inputChar)
        {
            if (inputChar == 8) // Backspace. Don't allow backspace on first character.
            {
                if(pos > 0)
                    HandleBackSpace();
                return true;
            }
            if (inputChar == SpaceChar) // Space. Don't allow space on first character.
            {
                if (pos > 0 && sb[pos - 1] != keywordDelim[0]) // Ignore multiple spaces.
                {
                    sb.Append(keywordDelim[0]);
                    pos++;
                }
                return true;
            }
            return false;
        }

        protected virtual bool CheckStandardCharacter(char inputChar)
        {
            if (alpha.IsMatch(inputChar.ToString())) // Normal character.
            {
                sb.Append(inputChar.ToString().ToLower());
                letters[pos++].text = inputChar.ToString();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Receive input to castbar (from Keyboard.cs).
        /// </summary>
        /// <param name="inputChar">Input character</param>
        public void CheckInput(char inputChar)
        {
            if (pos >= letters.Length - 1) // No more room.
            {
                Debug.Log("CastBar full");
                return;
            }
            if(CheckSpecialCharacter(inputChar) || CheckStandardCharacter(inputChar))
            {
                MatchKeywords(); // Find keywords and combos
            }
        }

        public void CheckInput(string inputString)
        {
            foreach (char inputChar in inputString)
                CheckInput(inputChar);
        }

        /// <summary>
        /// Submit current string in cast bar.
        /// </summary>
        public void Cast()
        {
            Debug.Log("CastBar:" + Text);
            onCast.Invoke(Text);
            pos = 0;
            Clear();
        }

        // Match keywords and apply effects.
        // Effects are attached as children to first letter in keyword.
        void MatchKeywords()
        {
            if (Text.Length <= 0)
                return;
            var keywords = sb.ToString().Split(keywordDelim);
            int pos = 0; // Character position.
            Keyword curr = null; // Current keyword.
            Keyword prev = null; // Previous keyword.
            for (int i = 0; i < keywords.Length; i++) // Word position.
            {
                if (i > 0) prev = curr;
                curr = keywordMap.ContainsKey(keywords[i])
                     ? keywordMap[keywords[i]].GetComponent<Keyword>()
                     : null;
                // Check if keyword matches. Don't re-add effects.
                if (keywordMap.ContainsKey(keywords[i]) && letters[pos].transform.childCount == 0)
                {
                    // Initial effect application.
                    Instantiate(keywordMap[keywords[i]], letters[pos].transform);
                    if (curr.type == Keyword.Type.root) // Root.
                    {
                        if (prev != null) // Check previous.
                        {
                            if (prev.type == Keyword.Type.left)
                            {
                                letters[pos - 1].text = Spell.separatorModRight.ToString();
                            }
                            else if (prev.type == Keyword.Type.root)
                            {
                                letters[pos - 1].text = Spell.separator.ToString();
                            }
                        }
                    }
                    else if (curr.type == Keyword.Type.right) // Right modifier.
                    {
                        if (prev != null) // Check previous.
                        {
                            if (prev.type == Keyword.Type.root)
                            {
                                letters[pos - 1].text = Spell.separatorModLeft.ToString();
                            }
                        }
                    }
                }
                else if (!keywordMap.ContainsKey(keywords[i])) // Invalid word
                {
                    // Remove broken effects.
                    if (letters[pos].transform.childCount > 0)
                    {
                        Destroy(letters[pos].transform.GetChild(0).gameObject);
                        if (pos > 0) letters[pos - 1].text = "";
                    }
                }
                pos += keywords[i].Length + 1;
            }
        }

        void Clear()
        {
            foreach (var letter in letters)
            {
                letter.text = "";
                foreach (Transform child in letter.transform)
                {
                    Destroy(child.gameObject);
                }
            }
            sb.Clear();
        }
    }
}
