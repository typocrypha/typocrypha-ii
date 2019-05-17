using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Typocrypha
{
    /// <summary>
    /// Manages an effect that applys to keys on keyboard.
    /// Should be attached to a key effect prefab.
    /// </summary>
    public abstract class KeyEffect : MonoBehaviour, IPausable
    {
        #region IPausable
        PauseHandle ph;
        public PauseHandle PH { get => ph; }
        public void OnPause(bool b) { }
        #endregion

        public Key key; // Affected keyboard key.

        void Awake()
        {
            ph = new PauseHandle(OnPause);
            Keyboard.instance.allEffects.Add(this);
        }

        /// <summary>
        /// Called when effect is first applied.
        /// </summary>
        abstract public void OnStart();

        /// <summary>
        /// Called when key is pressed.
        /// </summary>
        abstract public void OnPress();

        /// <summary>
        /// Reset key to normal state after effect ends.
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Removes the effect.
        /// </summary>
        public void Remove()
        {
            Reset();
            Keyboard.instance.allEffects.Remove(this);
            Destroy(gameObject);
        }
    }
}

