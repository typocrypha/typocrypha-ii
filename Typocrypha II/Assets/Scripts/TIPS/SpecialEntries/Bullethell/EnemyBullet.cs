using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bullethell
{
    /// <summary>
    /// Enemy projectile.
    /// </summary>
    public class EnemyBullet : MonoBehaviour
    {
        public float xspeed = 0.2f;
        public float yspeed = 0f;

        void Update()
        {
            transform.Translate(-xspeed, yspeed, 0);
        }
    }
}
