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

        [SerializeField] protected AudioClip sfxOverride;
        protected Key key; // Affected keyboard key.
        protected Caster caster; // Caster of effect, may be null

        void Awake()
        {
            ph = new PauseHandle(OnPause);     
        }

        public void Register(Key keyComponent, char key, Caster caster)
        {
            this.key = keyComponent;
            this.caster = caster;
            Keyboard.instance.allEffects.Add(key, this);
        }

        /// <summary>
        /// The number of keys an instance of this effect will affect
        /// </summary>
        public virtual int NumAffectedKeys { get => 1; }

        /// <summary>
        /// Called when effect is first applied.
        /// </summary>
        public virtual void OnStart()
        {
            key.OnPress += OnPress;
            key.SfxOverride = sfxOverride;
        }

        /// <summary>
        /// Called when key is pressed.
        /// </summary>
        abstract public void OnPress();

        /// <summary>
        /// Reset key to normal state after effect ends.
        /// </summary>
        public virtual void ResetEffect()
        {
            key.OnPress -= OnPress;
            key.SfxOverride = null;
            key.ResetOutputAndDisplay();
        }

        /// <summary>
        /// Removes the effect.
        /// </summary>
        public void Remove()
        {
            ResetEffect();
            Keyboard.instance.allEffects.Remove(key.Letter);
            MarkUnaffected(key.Letter);
            Destroy(gameObject);
        }

        /// <summary>
        /// Mark a character unaffected. should be done when cleaning up an effect
        /// </summary>
        /// <param name="c"> The char to mark unaffected (should be a-z) </param>
        protected void MarkUnaffected(char c)
        {
            Keyboard.instance.unaffectedKeys.Add(c);
        }
        /// <summary>
        /// Mark a character affected. should be done when starting an effect that affects more than 1 key
        /// </summary>
        /// <param name="c"> The char to mark affected (should be a-z) </param>
        protected void MarkAffected(char c)
        {
            Keyboard.instance.unaffectedKeys.Remove(c);
        }
    }
}

