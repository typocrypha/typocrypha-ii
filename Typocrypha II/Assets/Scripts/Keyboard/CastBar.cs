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
    public abstract class CastBar : MonoBehaviour, IInputHandler
    {
        protected static readonly Color promptColor = Color.gray;
        protected static readonly Color normalColor = Color.white;
        public PauseHandle PH { get; private set; }
        public void OnPause(bool b)
        {
            cursor.PH.Pause = b;
        }

        [SerializeField] private CastBarCursor cursor; // Cursor for keeping track of position.
        [SerializeField] protected TextMeshProUGUI[] letters; // Array of single letter rects.
        [SerializeField] protected AudioClip backspaceSfx;

        protected readonly StringBuilder sb = new StringBuilder(); // String builder for text.
        public string Text
        {
            get => sb.ToString();
        }

        protected string Prompt { get; set; } = string.Empty;

        protected int pos = 0; // Cursor position.
        readonly Regex alpha = new Regex("^[A-Za-z]"); // Matches alphabetic strings.
        public static char[] KeywordDelimiters { get; } = new char[] { '–', ' ', '-' };
        protected virtual char SpaceChar => ' ';

        protected virtual void Awake()
        {
            PH = new PauseHandle(OnPause);
        }

        protected virtual void Start()
        {
            Clear(false);
        }

        protected virtual void HandleBackSpace()
        {
            ClearLetter(--pos);
            sb.Remove(pos, 1);
            AudioManager.instance.PlaySFX(backspaceSfx);
        }

        protected void UpdateCursor(bool show)
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

        protected bool CheckBackspace(char inputChar, out bool? sfx)
        {
            if (inputChar == 8) // Backspace. Don't allow backspace on first character.
            {
                if (pos > 0)
                {
                    HandleBackSpace();
                    sfx = null;
                }
                else
                {
                    sfx = false;
                }
                return true;
            }
            else if(inputChar == 127) // Ctrl + backspace
            {
                if (pos > 0)
                {
                    Clear();
                    AudioManager.instance.PlaySFX(backspaceSfx);
                    sfx = null;
                }
                else
                {
                    sfx = false;
                }
                return true;
            }
            sfx = null;
            return false;
        }
        protected bool CheckSpace(char inputChar)
        {
            if (inputChar == SpaceChar) // Space. Don't allow space on first character.
            {
                if (pos > 0 && sb[pos - 1] != KeywordDelimiters[0]) // Ignore multiple spaces.
                {
                    sb.Append(KeywordDelimiters[0]);
                    SetLetter(pos++, KeywordDelimiters[0].ToString());
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
                SetLetter(pos++, inputChar.ToString());
                return true;
            }
            return false;
        }

        // Return null for no sfx, false for no input (fail) sfx, and true for key sfx
        public virtual bool? CheckInput(char inputChar)
        {
            if (CheckBackspace(inputChar, out var sfx))
            {
                UpdateCursor(true);
                return sfx;
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
        public abstract void Submit();

        protected void Clear(bool showCursor = true)
        {
            pos = 0;
            for (int i = 0; i < letters.Length; i++)
            {
                ClearLetter(i);
            }
            sb.Clear();
            UpdateCursor(showCursor);
        }

        protected void SetLetter(int index, string letter)
        {
            letters[index].text = letter;
            letters[index].color = normalColor;
        }

        protected void ClearLetter(int index)
        {
            if(index < Prompt.Length)
            {
                letters[index].text = Prompt[index].ToString();
                letters[index].color = promptColor;
            }
            else
            {
                letters[index].text = string.Empty;
            }
        }

        public virtual void Focus()
        {
            cursor.PH.Pause = false;
            UpdateCursor(true);
        }

        public virtual void Unfocus()
        {
            cursor.PH.Pause = true;
        }
    }
}
