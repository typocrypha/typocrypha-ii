using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdhocAnim
{
    /// <summary>
    /// Base class for adhoc animations.
    /// Used when animation is simple enough that full animator isn't needed.
    /// </summary>
    public abstract class AdhocAnimation : MonoBehaviour
    {
        public bool startOnStart = true; // Should animation start when object starts?

        void Start()
        {
            if (startOnStart) Play();
        }

        /// <summary>
        /// Plays animation.
        /// </summary>
        public abstract void Play();

        /// <summary>
        /// Stops animation.
        /// </summary>
        public abstract void Stop();
    }
}

