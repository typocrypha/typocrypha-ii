using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bullethell
{
    /// <summary>
    /// Manages background.
    /// </summary>
    public class Background : MonoBehaviour
    {
        public Transform scrolltr; // Transform of scrolling part of background.
        public float speed = 0.05f;

        void Update()
        {
            if (scrolltr.localPosition.x > -1f)
            {
                scrolltr.Translate(-speed, 0, 0);
            }
            else
            {
                scrolltr.localPosition = Vector3.zero;
            }
        }
    }
}

