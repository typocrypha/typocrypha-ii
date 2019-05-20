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
        // Stops input and pauses keyboard effects.
        public void OnPause(bool b)
        {
            enabled = !b;
            foreach (var effect in allEffects)
                effect.PH.Pause = b;
        }
        #endregion

        public static Keyboard instance = null;
        public List<GameObject> allEffectPrefabs;
        public CastBar castBar;
        public Dictionary<char, Key> keyMap; // Map from characters to keyboard keys.
        public Transform keys; // Object that holds all the key objects.
        public HashSet<KeyEffect> allEffects; // All active key effects on keyboard (managed by individual effects).

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
            allEffects = new HashSet<KeyEffect>();
            GetComponent<KeyboardBuilder>().BuildKeyboard(keys); // Construct keyboard.
            foreach(Transform key in keys) // Add keys to map.
                keyMap[key.gameObject.name[0]] = key.GetComponent<Key>();
        }

        // Check user input.
        void Update()
        {
            foreach (var c in keyMap) // Highlight pressed keys.
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
            foreach (var c in Input.inputString) // Add letters to cast bar.
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
            if (Input.GetKeyDown(KeyCode.Return)) // Cast if enter is pressed.
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
        /// <summary>
        /// Applies effect to list of keys by name.
        /// </summary>
        /// <param name="affected">List of characters effect.</param>
        /// <param name="effectName">Name of prefab of effect.</param>
        public void ApplyEffect(string affected, string effectName)
        {
            ApplyEffect(affected, allEffectPrefabs.Find(c => c.name == effectName));
        }
    }
}

