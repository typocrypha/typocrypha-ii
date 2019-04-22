using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Typocrypha
{
    /// <summary>
    /// Constructs keyboard interface.
    /// 
    /// Key order is constructed based on a format string:
    ///   [a-z] : Display that letter key.
    ///   [\n]  : Start a new row of keys.
    ///   [ {float} ] : Offset horizontally (space delimited).
    ///   
    /// Example formats:
    ///   qwerty: qwertyuiop\n 0.4 asdfghjkl\n 1.2 zxcvbnm
    ///   azerty: azertyuiop\nqsdfghjklm\n 1.4 wxcvbn
    /// </summary>
    public class KeyboardBuilder : MonoBehaviour
    {
        public GameObject keyPrefab; // Prefab for a single key.
        public float keySize = 0.6f; // Key size.
        public float hSpacing = 0.2f; // Horizontal spacing between keys.
        public float vSpacing = 0.2f; // Vertical spacing between rows.
        public const string keyboardFormat = // Key letter format.
            "qwertyuiop\n 0.4 asdfghjkl\n 1.2 zxcvbnm"; 

        /// <summary>
        /// Create keyboard by spawning key objects in correct locations.
        /// </summary>
        public void BuildKeyboard(Transform keys)
        { 
            Vector3 currPos = Vector2.zero;
            for (int i = 0; i < keyboardFormat.Length;)
            {
                char c = keyboardFormat[i];
                switch(c)
                {
                    case ' ': // Horizontal spacing.
                        int ppos = keyboardFormat.IndexOf(' ', i+1);
                        currPos.x += float.Parse(keyboardFormat.Substring(i, ppos - i));
                        i = ppos + 1;
                        break;
                    case '\n': // Start a new row.
                        currPos.x = 0;
                        currPos.y -= keySize + vSpacing;
                        i++;
                        break;
                    default: // Alphabetical character.
                        var go = Instantiate(keyPrefab, keys);
                        go.name = c.ToString();
                        go.transform.localPosition = currPos;
                        go.transform.localScale = Vector2.one * keySize;
                        Key key = go.GetComponent<Key>();
                        key.letter = c;
                        key.output = c.ToString();
                        key.letterText.text = c.ToString().ToUpper();
                        currPos.x += keySize + hSpacing;
                        i++;
                        break;
                }
            }
        }
    }
}

