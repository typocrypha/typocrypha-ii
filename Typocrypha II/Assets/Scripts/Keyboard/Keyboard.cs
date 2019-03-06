using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Typocrypha
{
    /// <summary>
    /// Manages keyboard interface.
    /// </summary>
    [RequireComponent(typeof(KeyboardBuilder))]
    public class Keyboard : MonoBehaviour
    {
        public static Keyboard instance = null;
        public Dictionary<char, Key> keyMap; // Map from characters to keyboard keys.
        public TMP_InputField inputBar; // Input field for typing.

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);

            keyMap = new Dictionary<char, Key>();
            GetComponent<KeyboardBuilder>().BuildKeyboard(); // Construct keyboard.
            foreach(Transform key in transform.Find("Keys")) // Add keys to map.
                keyMap[key.gameObject.name[0]] = key.GetComponent<Key>();
        }

        string frameInput = "";

        // Check use input.
        void Update()
        {
            /*
            frameInput = Input.inputString;
            foreach (var c in keyMap.Keys)
            {
                
                if (frameInput.Contains(c.ToString()))
                {
                    keyMap[c].onPress();
                    keyMap[c].Highlight(true);
                }
                else
                {
                    keyMap[c].Highlight(false);
                }
            }
            */
            foreach (var c in keyMap)
            {
                if (Input.GetKey(c.Key.ToString()))
                {
                    c.Value.Highlight(true);
                }
                else
                {
                    c.Value.Highlight(false);
                }
            }
        }

        /// <summary>
        /// Called when input bar is submitted. (Handle set in editor).
        /// </summary>
        /// <param name="inputString">Submitted string.</param>
        public void OnSubmit(string inputString)
        {
            Debug.Log(inputString);
        }
    }
}

