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
        public float speed = 0.2f;

        void Update()
        {
            transform.Translate(-speed, 0, 0);
        }
    }
}
