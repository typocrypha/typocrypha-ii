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
        [SerializeField] private OverheatManager overheatManager;
        public CasterUI PlayerUI => playerUI;
        [SerializeField] private CasterUI playerUI;
        public readonly Dictionary<char, Key> keyMap = new Dictionary<char, Key>(); // Map from characters to keyboard keys.
        public readonly Dictionary<char, KeyEffect> allEffects = new Dictionary<char, KeyEffect>(); // All active key effects on keyboard (managed by individual effects).
        public readonly HashSet<char> unaffectedKeys = new HashSet<char>(); // All keys not currently affected by a key effect
        private static readonly Dictionary<char, KeyCode> keyCodeMap = new Dictionary<char, KeyCode>()
        {
            {'a', KeyCode.A }, {'b', KeyCode.B }, {'c', KeyCode.C }, {'d', KeyCode.D }, {'e', KeyCode.E },
            {'f', KeyCode.F }, {'g', KeyCode.G }, {'h', KeyCode.H }, {'i', KeyCode.I }, {'j', KeyCode.J },
            {'k', KeyCode.K }, {'l', KeyCode.L }, {'m', KeyCode.M }, {'n', KeyCode.N }, {'o', KeyCode.O },
            {'p', KeyCode.P }, {'q', KeyCode.Q }, {'r', KeyCode.R }, {'s', KeyCode.S }, {'t', KeyCode.T },
            {'u', KeyCode.U }, {'v', KeyCode.V }, {'w', KeyCode.W }, {'x', KeyCode.X }, {'y', KeyCode.Y },
            {'z', KeyCode.Z },
            {'0', KeyCode.Alpha0 }, {'1', KeyCode.Alpha1 }, {'2', KeyCode.Alpha2 }, {'3', KeyCode.Alpha3 },
            {'4', KeyCode.Alpha4 }, {'5', KeyCode.Alpha5 }, {'6', KeyCode.Alpha6 }, {'7', KeyCode.Alpha7 },
            {'8', KeyCode.Alpha8 }, {'9', KeyCode.Alpha9 },
        };
        private static readonly Dictionary<char, KeyCode> secondaryKeyCodeMap = new Dictionary<char, KeyCode>()
        {
            {'0', KeyCode.Keypad0 }, {'1', KeyCode.Keypad1 }, {'2', KeyCode.Keypad2 }, {'3', KeyCode.Keypad3 },
            {'4', KeyCode.Keypad4 }, {'5', KeyCode.Keypad5 }, {'6', KeyCode.Keypad6 }, {'7', KeyCode.Keypad7 },
            {'8', KeyCode.Keypad8 }, {'9', KeyCode.Keypad9 },
        };

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
                keyMap[key.Letter] = key;
                // Initialize unaffected key set
                unaffectedKeys.Add(key.Letter);
            }
        }

        private bool GetKey(char key)
        {
            return Input.GetKey(keyCodeMap[key]) || (secondaryKeyCodeMap.TryGetValue(key, out var secondaryCode) && Input.GetKey(secondaryCode));
        }

        // Check user input.
        void Update()
        {
            if (overheatManager.IsOverheating)
                return;
            foreach (var c in keyMap) // Highlight pressed keys.
            {
                c.Value.Highlight = GetKey(c.Key);
            }
            foreach (var c in Input.inputString) // Add letters to cast bar.
            {
                if (keyMap.ContainsKey(c))
                {
                    keyMap[c].OnPress?.Invoke();
                    castBar.CheckInput(keyMap[c].Output);
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
        /// Removes all key effects and stops overheat
        /// </summary>
        public void ClearKeyEffects()
        {
            foreach(var key in allEffects.Keys.ToArray())
            {
                allEffects[key].Remove();
            }
        }

        public void Clear()
        {
            ClearKeyEffects();
            overheatManager.StopOverheat();
            foreach (var c in keyMap) // Turn all highlights off
            {
                c.Value.Highlight = false;
            }
        }

        public void DoOverheat()
        {
            overheatManager.DoOverheat();
        }
    }
}

