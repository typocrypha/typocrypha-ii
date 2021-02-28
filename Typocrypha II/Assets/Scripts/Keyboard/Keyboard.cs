using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

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
            foreach (var kvp in allEffects)
                kvp.Value.PH.Pause = b;
        }
        #endregion

        public const char randomKeyFail = '!';
        public static Keyboard instance = null;
        public List<GameObject> allEffectPrefabs;
        public CastBar castBar;
        public Transform keys; // Object that holds all the key objects.
        public readonly Dictionary<char, Key> keyMap = new Dictionary<char, Key>(); // Map from characters to keyboard keys.
        public readonly Dictionary<char, KeyEffect> allEffects = new Dictionary<char, KeyEffect>(); // All active key effects on keyboard (managed by individual effects).
        public readonly HashSet<char> unaffectedKeys = new HashSet<char>(); // All keys not currently affected by a key effect

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

            var builder = GetComponent<KeyboardBuilder>(); // Construct keyboard.
            builder.BuildKeyboard();
            Initialize(builder.Keys);
        }

        public void Initialize(IEnumerable<Key> keys)
        {
            keyMap.Clear();
            unaffectedKeys.Clear();
            foreach (Key key in keys)
            {
                // Add keys to map.
                keyMap[key.letter] = key;
                // Initialize unaffected key set
                unaffectedKeys.Add(key.letter);
            }
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
                if (!unaffectedKeys.Contains(c))
                    continue;
                keyMap[c].ApplyEffect(effectPrefab);
                unaffectedKeys.Remove(c);
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
        /// <summary>
        /// Apply an effect to a random key x times.
        /// </summary>
        /// <param name="effectPrefab">Prefab of effect (contains functionality and visual/audio effects.</param>
        /// <param name="times">Number of times to attempt application.</param>
        /// <returns>The number of times the effect was sucessfully applied</returns>
        public string ApplyEffectRandom(GameObject effectPrefab, int times = 1)
        {
            int numKeysPerEffect = effectPrefab.GetComponent<KeyEffect>().NumAffectedKeys;
            string affected = string.Empty;
            for (int i = 0; i < times; ++i)
            {
                if (numKeysPerEffect > unaffectedKeys.Count)
                    return affected;
                char key = GetRandomUnaffectedKey();
                ApplyEffect(key.ToString(), effectPrefab);
                affected += key;
            }
            return affected;
        }
        /// <summary>
        /// Apply an effect to a random key x times by an effect name.
        /// </summary>
        /// <param name="effectName">Name of prefab of effect.</param>
        /// <param name="times">Number of times to attempt application.</param>
        /// <returns></returns>
        public string ApplyEffectRandom(string effectName, int times = 1)
        {
            return ApplyEffectRandom(allEffectPrefabs.Find(c => c.name == effectName), times);
        }
        /// <summary>
        /// Returns a random, unaffected key.
        /// If there are no unaffected keys, returns '!'
        /// </summary>
        /// <returns> The key, or '!' if none exist </returns>
        public char GetRandomUnaffectedKey()
        {
            if (unaffectedKeys.Count <= 0)
                return randomKeyFail;
            return RandomUtils.RandomU.instance.Choice(unaffectedKeys);
        }
        /// <summary>
        /// Remove an effect from one or more keys
        /// </summary>
        /// <param name="keys">each character in this string will have its effect removed</param>
        public void RemoveEffect(string keys)
        {
            foreach (char c in keys)
            {
                if (unaffectedKeys.Contains(c))
                    continue;
                allEffects[c].Remove();
            }
        }

        /// <summary>
        /// Removes all key effects
        /// </summary>
        public void Clear()
        {
            foreach(var key in allEffects.Keys.ToArray())
            {
                allEffects[key].Remove();
            }
        }
    }
}

