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
        public delegate void OnPressDel(); // Delegate for when key is pressed.

        public char letter; // Letter this key represents.
        public string output; // What is typed when this key is pressed.

        public Image highlightSR; // Sprite renderer for key highlight.
        public TextMeshProUGUI letterText; // Text for key label.
        float highlight = 0f; // Normalized highlight value.
        Color originalColor;

        public System.Action OnPress { get; set; } // Delegate called when key is pressed.

        [SerializeField] private AudioClip defaultKeySfx;

        public AudioClip SfxOverride { get; set; }
        
        public bool Highlight // Key highlight (when pressed).
        {
            set
            {
                if (value) highlight = 1f;
                else if (highlight >= 0f) highlight -= 0.15f;
            }
        }

        void Awake()
        {
            Highlight = false;
        }

        private void Start()
        {
            originalColor = new Color(highlightSR.color.r, highlightSR.color.g, highlightSR.color.b, 1.0f);
            OnPress += PlaySfx;
        }

        void Update()
        {
            highlightSR.color = originalColor * highlight;
        }

        public void PlaySfx()
        {
            AudioManager.instance.PlaySFX(SfxOverride ?? defaultKeySfx);
        }

        public virtual void SetText(char c)
        {
            letter = c;
            output = c.ToString();
            letterText.text = output.ToUpper();
        }

        /// <summary>
        /// Apply a key effect to this key.
        /// </summary>
        /// <param name="effectPrefab">Key effect prefab.</param>
        public void ApplyEffect(GameObject effectPrefab)
        {
            var effect = Instantiate(effectPrefab, transform).GetComponent<KeyEffect>();
            effect.transform.localPosition = Vector3.zero;
            effect.Register(this, letter);
            effect.OnStart();
        }
    }
}

