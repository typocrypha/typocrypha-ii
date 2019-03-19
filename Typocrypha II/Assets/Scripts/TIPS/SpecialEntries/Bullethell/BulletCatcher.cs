using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bullethell
{
    /// <summary>
    /// Catches and destroys bullets/enemies that go off screen.
    /// </summary>
    public class BulletCatcher : MonoBehaviour
    {
        public void OnTriggerEnter2D(Collider2D col)
        {
            if (col.GetComponent<Bullet>() != null ||
                col.GetComponent<EnemyBullet>() != null)
                Destroy(col.gameObject);
        }
    }
}
