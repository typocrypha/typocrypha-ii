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
        public GameObject keyPrefabAlt; // Alt color key prefab.
        public Transform[] rows; // Row transforms.
        public const string keyboardFormat = // Key letter format.
            "qwertyuiop\n asdfghjkl\n zxcvbnm"; 

        /// <summary>
        /// Create keyboard by spawning key objects in correct locations.
        /// </summary>
        public void BuildKeyboard()
        {
            int rowind = 0;
            bool colorSwap = false;
            Vector3 currPos = Vector2.zero;
            for (int i = 0; i < keyboardFormat.Length; i++)
            {
                GameObject go = null;
                char c = keyboardFormat[i];
                switch(c)
                {
                    case ' ': // Color swap.
                        colorSwap = !colorSwap;
                        break;
                    case '\n': // Start a new row.
                        rowind++;
                        break;
                    default: // Alphabetical character.
                        if (colorSwap = !colorSwap)
                            go = Instantiate(keyPrefab, rows[rowind]);
                        else
                            go = Instantiate(keyPrefabAlt, rows[rowind]);
                        go.name = c.ToString();
                        go.transform.localScale = Vector2.one;
                        Key key = go.GetComponent<Key>();
                        key.letter = c;
                        key.output = c.ToString();
                        key.letterText.text = c.ToString().ToUpper();
                        break;
                }
            }
        }
    }
}

