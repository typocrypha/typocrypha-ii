using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Typocrypha
{
    /// <summary>
    /// Manages a single key on the keyboard.
    /// </summary>
    public class Key : MonoBehaviour
    {
        [SerializeField] private Image highlightImage; // Sprite renderer for key highlight.
        [SerializeField] private TextMeshProUGUI letterText; // Text for key label.
        public Transform KeyEffectContainer => keyEffectContainer;
        [SerializeField] Transform keyEffectContainer;

        public char Letter => letter;
        private char letter; // Letter this key represents.
        public IReadOnlyList<char> Output => output;
        private readonly List<char> output = new List<char>(4); // What is typed when this key is pressed.
        private float highlight = 0f; // Normalized highlight value.
        private Color originalColor;


        public System.Action OnPress { get; set; } // Delegate called when key is pressed.

        [SerializeField] private AudioClip defaultKeySfx;

        public AudioClip SfxOverride { get; set; }
        
        public bool Highlight // Key highlight (when pressed).
        {
            set
            {
                float curr = highlight;
                // Modify highlight
                if (value)
                {
                    highlight = 1f;
                }
                else if (highlight >= 0f)
                {
                    
                    highlight = Mathf.Max(highlight - 0.15f, 0);
                }
                // Set color if necessary
                if(highlight != curr)
                {
                    highlightImage.color = originalColor * highlight;
                }
            }
        }

        public void ClearHighlight()
        {
            if (highlight == 0)
                return;
            highlight = 0;
            highlightImage.color = originalColor * highlight;
        }

        void Awake()
        {
            originalColor = new Color(highlightImage.color.r, highlightImage.color.g, highlightImage.color.b, 1.0f);
            Highlight = false;
        }

        public void PlaySfx()
        {
            AudioManager.instance.PlaySFX(SfxOverride ?? defaultKeySfx);
        }

        public virtual void SetLetter(char c)
        {
            letter = c;
            SetOutputAndDisplay(c);
        }

        public void SetDisplay(char c)
        {
            SetDisplay(c.ToString());
        }

        public void SetDisplay(string s)
        {
            letterText.text = s.ToUpper();
        }

        public void ClearOutput()
        {
            output.Clear();
        }

        public void SetOutput(char c)
        {
            output.Clear();
            output.Add(c);
        }

        public void AddToOutput(char c)
        {
            output.Add(c);
        }

        public void SetOutput(IEnumerable<char> c)
        {
            output.Clear();
            output.AddRange(c);
        }

        public void SetOutputAndDisplay(char c)
        {
            SetOutput(c);
            SetDisplay(c);
        }

        public void ResetOutputAndDisplay()
        {
            SetOutputAndDisplay(letter);
        }

        /// <summary>
        /// Apply a key effect to this key.
        /// </summary>
        /// <param name="effectPrefab">Key effect prefab.</param>
        public void ApplyEffect(GameObject effectPrefab)
        {
            var effect = Instantiate(effectPrefab, keyEffectContainer).GetComponent<KeyEffect>();
            effect.transform.localPosition = Vector3.zero;
            effect.Register(this, letter);
            effect.OnStart();
        }
    }
}

