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
        public SpriteRenderer highlight; // Sprite renderer for key highlight.
        public TextMeshPro letterText; // Text for key label.
        public bool Highlight // Key highlight (when pressed).
        {
            set
            {
                if (value) highlight.color = Color.clear;
                else highlight.color = Color.black * 0.2f; 
            }
        }
        public bool Affected // Under status effect (checks for effect child).
        {
            get => transform.childCount != 3;
        }

        void Awake()
        {
            Highlight = false;
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

