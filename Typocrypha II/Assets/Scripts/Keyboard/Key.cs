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
        public delegate string OnPressDel(); // Delegate for when key is pressed.

        public OnPressDel onPress; // Delegate called when key is pressed.
        public SpriteRenderer sr; // Sprite renderer for key background.
        public TextMeshPro tmp; // Text for key label.

        void Awake()
        {
            onPress += NullAction;
            Highlight(false);
        }

        string NullAction() => "";

        /// <summary>
        /// Highlight key on/off.
        /// </summary>
        /// <param name="on">If on==false, then shade. Otherwise, show as normal.</param>
        public void Highlight(bool on)
        {
            if (on)
            {
                sr.color = Color.white;
            }
            else
            {
                sr.color = Color.gray;
            }
        }
    }
}

