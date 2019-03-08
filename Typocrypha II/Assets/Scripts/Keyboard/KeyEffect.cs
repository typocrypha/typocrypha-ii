using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Typocrypha
{
    /// <summary>
    /// Manages an effect that applys to keys on keyboard.
    /// Should be attached to a key effect prefab.
    /// </summary>
    public abstract class KeyEffect : MonoBehaviour
    {
        public Key key; // Affected keyboard key.

        /// <summary>
        /// Called when effect is first applied.
        /// </summary>
        abstract public void OnStart();

        /// <summary>
        /// Called when key is pressed.
        /// </summary>
        abstract public void OnPress();

        /// <summary>
        /// Remove the effect.
        /// </summary>
        abstract public void Remove();
    }
}

