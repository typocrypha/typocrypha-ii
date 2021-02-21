using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Linq;

namespace Typocrypha
{
    public class CastBarPhone : CastBar
    {
        public const float waitTime = 0.5f;
        private const char nullChar = '?';
        private const char spaceChar = '0';
        protected override char SpaceChar => spaceChar;
        private readonly Regex numeric = new Regex("^[2-9]"); // Matches numric strings.
        private static readonly Dictionary<char, char[]> mapping = new Dictionary<char, char[]>()
        {

            {'2', new char[]{'a','b','c'} },
            {'3', new char[]{'d','e','f'} },
            {'4', new char[]{'g','h','i'} },
            {'5', new char[]{'j','k','l'} },
            {'6', new char[]{'m','n','o'} },
            {'7', new char[]{'p','q','r','s'} },
            {'8', new char[]{'t','u','v'} },
            {'9', new char[]{'w','x','y','z'} },
        };
        private int index = 0;
        private char lastKey = nullChar;
        private Coroutine waitCr = null;

        public static string GetToolTipText(char c)
        {
            if (c == spaceChar)
                return "_";
            if (mapping.ContainsKey(c))
                return string.Concat(mapping[c].Select(char.ToString));
            return string.Empty;
        }

        protected override void HandleBackSpace()
        {
            base.HandleBackSpace();
            lastKey = nullChar;
            index = 0;
            if (waitCr != null)
            {
                StopCoroutine(waitCr);
            }
        }

        protected override bool CheckStandardCharacter(char inputChar)
        {
            if(!numeric.IsMatch(inputChar.ToString()))
                return false;

            if(inputChar != lastKey) // new key
            {
                index = 0;
                lastKey = inputChar;
                string newLetter = mapping[lastKey][index].ToString();
                letters[pos++].text = newLetter;
                sb.Append(newLetter);
            }
            else // repeat key
            {
                var mappedLetters = mapping[lastKey];
                if (++index >= mappedLetters.Length)
                    index = 0;
                char newChar = mappedLetters[index];
                letters[pos - 1].text = newChar.ToString();
                sb[pos - 1] = newChar;
            }

            // Wait and reset the letter
            if(waitCr != null)
            {
                StopCoroutine(waitCr);
            }
            waitCr = StartCoroutine(WaitForPause(lastKey));
            return true;
        }

        private IEnumerator WaitForPause(char initialKey)
        {
            yield return new WaitForSeconds(waitTime);
            if(lastKey == initialKey)
            {
                index = 0;
                lastKey = nullChar;
            }
        }
    }
}
