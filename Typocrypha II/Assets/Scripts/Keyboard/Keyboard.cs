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

        // Check user input.
        void Update()
        {
            foreach (var c in keyMap)
            {
                if (Input.GetKey(c.Key.ToString()))
                {
                    c.Value.Highlight = true;
                }
                else
                {
                    c.Value.Highlight = false;
                }
            }
            foreach (var c in Input.inputString)
            {
                keyMap[c].onPress?.Invoke();
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

        /// <summary>
        /// Apply an effect to a list of keys.
        /// </summary>
        /// <param name="affected">List of characters effect.</param>
        /// <param name="effectPrefab">Prefab of effect (contains functionality and visual/audio effects.</param>
        public void ApplyEffect(string affected, GameObject effectPrefab) 
        {
            foreach(char c in affected)
            {
                keyMap[c].ApplyEffect(effectPrefab);
            }
        }
    }
}

