using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bullethell
{
    /// <summary>
    /// Bullet explodes and spreads into 3 bullets.
    /// </summary>
    public class EnemyBulletSpread : EnemyBullet
    {
        public GameObject bulletPrefab; // Prefab of spread bullet.

        IEnumerator Start()
        {
            yield return new WaitForSeconds(0.5f);
            // Upper bullet
            var go1 = Instantiate(bulletPrefab, transform.parent);
            go1.transform.position = transform.position;
            go1.GetComponent<EnemyBullet>().yspeed = 0.05f;
            // Middle bullet
            var go2 = Instantiate(bulletPrefab, transform.parent);
            go2.transform.position = transform.position;
            // Lower bullet
            var go3 = Instantiate(bulletPrefab, transform.parent);
            go3.transform.position = transform.position;
            go3.GetComponent<EnemyBullet>().yspeed = -0.05f;

            Destroy(gameObject);
        }
    }
}

