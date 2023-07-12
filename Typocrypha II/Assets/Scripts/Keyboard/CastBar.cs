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
        protected const int maxLetters = 30;
        protected static readonly Color promptColor = Color.gray;
        protected static readonly Color normalColor = Color.white;
        protected static readonly Color wrongColor = Color.red;
        public PauseHandle PH { get; private set; }
        public void OnPause(bool b)
        {
            cursor.PH.Pause = b;
        }

        [SerializeField] private CastBarCursor cursor; // Cursor for keeping track of position.
        [SerializeField] private TextMeshProUGUI[] letters; // Array of single letter rects.
        [SerializeField] protected AudioClip backspaceSfx;


        protected List<TextMeshProUGUI> ActiveLetters { get; } = new List<TextMeshProUGUI>(maxLetters);
        protected readonly StringBuilder sb = new StringBuilder(maxLetters); // String builder for text.
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
            if(ActiveLetters.Count <= 0)
            {
                Resize(letters.Length);
            }
            Clear(false);
        }

        public void Resize(int size)
        {
            ActiveLetters.Clear();
            int newSize = Mathf.Min(size, letters.Length);
            for (int i = 0; i < letters.Length; i++)
            {
                if (i < newSize)
                {
                    ActiveLetters.Add(letters[i]);
                    letters[i].gameObject.SetActive(true);
                }
                else
                {
                    letters[i].gameObject.SetActive(false);
                }
            }
            if(pos >= newSize)
            {
                pos = newSize - 1;
            }
        }

        protected virtual void HandleBackSpace()
        {
            ClearLetter(--pos);
            sb.Remove(pos, 1);
            AudioManager.instance.PlaySFX(backspaceSfx);
        }

        protected void UpdateCursor(bool show)
        {
            if (pos >= ActiveLetters.Count)
            {
                cursor.gameObject.SetActive(false);
                return;
            }
            if (!cursor.gameObject.activeInHierarchy)
            {
                cursor.gameObject.SetActive(true);
            }
            cursor.transform.position = ActiveLetters[pos].transform.position;
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
                    Clear(true);
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
                    SetLetter(pos++, KeywordDelimiters[0]);
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
                SetLetter(pos++, inputChar);
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
            if (pos >= ActiveLetters.Count) // No more room.
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

        protected void Clear(bool showCursor)
        {
            pos = 0;
            for (int i = 0; i < ActiveLetters.Count; i++)
            {
                ClearLetter(i);
            }
            sb.Clear();
            UpdateCursor(showCursor);
        }

        protected void SetLetter(int index, char letter)
        {
            ActiveLetters[index].text = letter.ToString();
            if(IsLetterWrong(index, letter))
            {
                ActiveLetters[index].color = wrongColor;
            }
            else
            {
                ActiveLetters[index].color = normalColor;
            }
        }

        protected virtual bool IsLetterWrong(int index, char letter)
        {
            return index < Prompt.Length && char.ToLower(letter) != char.ToLower(Prompt[index]);
        }

        protected void ClearLetter(int index)
        {
            if(index < Prompt.Length)
            {
                ActiveLetters[index].text = Prompt[index].ToString();
                ActiveLetters[index].color = promptColor;
            }
            else
            {
                ActiveLetters[index].text = string.Empty;
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
