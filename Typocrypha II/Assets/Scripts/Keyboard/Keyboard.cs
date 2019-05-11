using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Typocrypha
{
    /// <summary>
    /// Manages keyboard interface for battle.
    /// </summary>
    [RequireComponent(typeof(KeyboardBuilder))]
    public class Keyboard : MonoBehaviour, IPausable
    {
        #region IPausable
        PauseHandle ph;
        public PauseHandle PH { get => ph; }
        public void OnPause(bool b)
        {
            enabled = !b;
        }
        #endregion

        public static Keyboard instance = null;
        public CastBar castBar;
        public Dictionary<char, Key> keyMap; // Map from characters to keyboard keys.
        public Transform keys; // Object that holds all the key objects.

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
            ph = new PauseHandle(OnPause);

            keyMap = new Dictionary<char, Key>();
            GetComponent<KeyboardBuilder>().BuildKeyboard(keys); // Construct keyboard.
            foreach(Transform key in keys) // Add keys to map.
                keyMap[key.gameObject.name[0]] = key.GetComponent<Key>();
        }

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
                var text = castBar.Text;
                if (keyMap.ContainsKey(c))
                {
                    keyMap[c].onPress?.Invoke();
                    castBar.CheckInput(keyMap[c].output);
                }
                else
                {
                    castBar.CheckInput(c);
                }
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                castBar.Cast();
            }
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

