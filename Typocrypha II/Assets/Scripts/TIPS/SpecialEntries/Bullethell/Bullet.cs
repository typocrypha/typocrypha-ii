using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bullethell
{
    /// <summary>
    /// Bullet behaviour (the one that's shot from Bullet (Dog)).
    /// </summary>
    public class Bullet : MonoBehaviour
    {
        public float speed = 0.25f;

        void Update()
        {
            transform.Translate(speed, 0, 0);
        }
    }
}
