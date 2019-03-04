using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }
}

