using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
        public const string keyboardFormatQwerty = "qwertyuiop\n asdfghjkl\n zxcvbnm"; // Key letter format.
        public const string keyboardFormatDvorak = "pyfgcrl\n aoeuidhtns\n  qjkxbmwvs"; // Key letter format.
        public const string keyboardFormatColemak = "qwfpgjluy\n arstdhneio\n zxcvbkm"; // Key letter format.
        public const string keyboardFormatPhone = "1234567890"; // Key letter format.
        public const string keyboardFormatNumPad = "789\n 456\n 1230"; // Key letter format.
        public enum Mode
        {
            Keyboard,
            Phone,
        }
        public GameObject keyPrefab; // Prefab for a single key.
        public GameObject keyPrefabAlt; // Alt color key prefab.
        public Transform[] rows; // Row transforms.
        public Mode mode; // Keyboard mode
        public IEnumerable<Key> Keys => keyObjects.Select(obj => obj.GetComponent<Key>());
        private readonly List<GameObject> keyObjects = new List<GameObject>(); // Key objects

        private string KeyboardFormat
        {
            get
            {
                if(mode == Mode.Keyboard)
                {
                    switch (Settings.KeyLayout)
                    {
                        case Settings.KeyLayoutType.CUSTOM:
                            return Settings.CustomKeyLayout;
                        case Settings.KeyLayoutType.QWERTY:
                            return keyboardFormatQwerty;
                        case Settings.KeyLayoutType.DVORAK:
                            return keyboardFormatDvorak;
                        case Settings.KeyLayoutType.COLEMAK:
                            return keyboardFormatColemak;
                        default:
                            goto case Settings.KeyLayoutType.QWERTY;
                    }
                }
                else // Mode == phone
                {
                    return keyboardFormatPhone;
                }
            }
        }
        /// <summary>
        /// Create keyboard by spawning key objects in correct locations.
        /// </summary>
        public void BuildKeyboard()
        {
            ClearKeyboard();
            int rowind = 0;
            bool colorSwap = false;
            Vector3 currPos = Vector2.zero;
            string format = KeyboardFormat;
            for (int i = 0; i < format.Length; i++)
            {

                char c = format[i];
                switch(c)
                {
                    case ' ': // Color swap.
                        colorSwap = !colorSwap;
                        break;
                    case '\n': // Start a new row.
                        rowind++;
                        break;
                    default: // Alphabetical character.
                        GameObject go = Instantiate((colorSwap = !colorSwap) ? keyPrefab : keyPrefabAlt, rows[rowind]);
                        go.name = c.ToString();
                        go.transform.localScale = Vector2.one;
                        go.GetComponent<Key>()?.SetText(c);
                        keyObjects.Add(go);
                        break;
                }
            }
        }

        public void ClearKeyboard()
        {
            foreach (var obj in keyObjects)
            {
                Destroy(obj);
            }
            keyObjects.Clear();
        }
    }
}

