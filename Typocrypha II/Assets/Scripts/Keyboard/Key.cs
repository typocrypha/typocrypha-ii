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
        public TextMeshPro tmp; // Text for key label.
    }
}

