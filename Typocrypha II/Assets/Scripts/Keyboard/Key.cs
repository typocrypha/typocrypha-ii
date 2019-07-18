using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
        public OnPressDel onPress; // Delegate called when key is pressed.
        public SpriteRenderer highlightSR; // Sprite renderer for key highlight.
        public TextMeshPro letterText; // Text for key label.
        float highlight = 0f; // Normalized highlight value.
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

        void Update()
        {
            highlightSR.color = Color.white * highlight;
        }

        /// <summary>
        /// Apply a key effect to this key.
        /// </summary>
        /// <param name="effectPrefab">Key effect prefab.</param>
        public void ApplyEffect(GameObject effectPrefab)
        {
            var go = Instantiate(effectPrefab, transform);
            go.transform.localPosition = Vector3.zero;
            var effect = go.GetComponent<KeyEffect>();
            effect.key = this;
            onPress += effect.OnPress;
            effect.OnStart();
        }
    }
}

