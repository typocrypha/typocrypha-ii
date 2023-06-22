using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;

namespace Typocrypha
{
    /// <summary>
    /// Manages Cast Bar interface for battle.
    /// </summary>
    public class CastBar : MonoBehaviour
    {
        public CastBarCursor cursor; // Cursor for keeping track of position.
        [SerializeField] protected TextMeshProUGUI[] letters; // Array of single letter rects.
        [SerializeField] protected AudioClip backspaceSfx;

        protected readonly StringBuilder sb = new StringBuilder(); // String builder for text.
        public string Text
        {
            get => sb.ToString();
        }

        protected int pos = 0; // Cursor position.
        readonly Regex alpha = new Regex("^[A-Za-z]"); // Matches alphabetic strings.
        public static char[] KeywordDelimiters { get; } = new char[] { '–', ' ', '-' };
        protected virtual char SpaceChar => ' ';

        void Start()
        {
            Clear(false);
        }

        protected virtual void HandleBackSpace()
        {
            letters[--pos].text = "";
            sb.Remove(pos, 1);
            AudioManager.instance.PlaySFX(backspaceSfx);
        }

        private void UpdateCursor(bool show)
        {
            if (pos >= letters.Length)
            {
                cursor.gameObject.SetActive(false);
                return;
            }
            if (!cursor.gameObject.activeInHierarchy)
            {
                cursor.gameObject.SetActive(true);
            }
            cursor.transform.position = letters[pos].transform.position;
            cursor.SetDelay(0.2f, show);
        }

        private bool CheckBackspace(char inputChar)
        {
            if (inputChar == 8) // Backspace. Don't allow backspace on first character.
            {
                if (pos > 0)
                {
                    HandleBackSpace();
                }
                return true;
            }
            else if(inputChar == 127) // Ctrl + backspace
            {
                if (pos > 0)
                {
                    Clear();
                    AudioManager.instance.PlaySFX(backspaceSfx);
                }
                return true;
            }
            return false;
        }
        private bool CheckSpace(char inputChar)
        {
            if (inputChar == SpaceChar) // Space. Don't allow space on first character.
            {
                if (pos > 0 && sb[pos - 1] != KeywordDelimiters[0]) // Ignore multiple spaces.
                {
                    sb.Append(KeywordDelimiters[0]);
                    letters[pos++].text = KeywordDelimiters[0].ToString();
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

        // Return null for no sfx, false for no input (fail) sfx, and true for key sfx
        public bool? CheckInput(char inputChar)
        {
            if (CheckBackspace(inputChar))
            {
                UpdateCursor(true);
                return null;
            }
            if (pos >= letters.Length) // No more room.
            {
                return false;
            }
            if(CheckSpace(inputChar) || CheckStandardCharacter(inputChar))
            {
                UpdateCursor(true);
                return true;
            }
            return false;
        }

        // Return null for no sfx, false for no input (fail) sfx, and true for key sfx
        public bool? CheckInput(IEnumerable<char> input)
        {
            bool? playKeySfx = null;
            foreach (char inputChar in input)
            {
                var latestInput = CheckInput(inputChar);
                if(latestInput.HasValue)
                {
                    if (playKeySfx.HasValue)
                    {
                        playKeySfx = playKeySfx.Value || latestInput.Value;
                    }
                    else
                    {
                        playKeySfx = latestInput;
                    }
                }
            }
            return playKeySfx;
        }

        /// <summary>
        /// Submit current string in cast bar.
        /// </summary>
        public void Cast()
        {
            if(Battlefield.instance.Player is Player player)
            {
                player.CastString(Text.TrimEnd(KeywordDelimiters).Split(KeywordDelimiters));
            }
            else
            {
                Debug.LogError("Player is not valid. Cannot cast");
            }
            Clear(false);
        }

        void Clear(bool showCursor = true)
        {
            pos = 0;
            foreach (var letter in letters)
            {
                letter.text = "";
            }
            sb.Clear();
            UpdateCursor(showCursor);
        }
    }
}
