using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bullethell
{
    /// <summary>
    /// Controls Bullet the dog during the 'Bullethell' TIPS minigame.
    /// </summary>
    public class BulletTheDog : MonoBehaviour
    {
        public GameObject bulletPrefab; // Shot projectile prefab.
        public SpriteRenderer sr; // Bullet's sprite renderer.
        public AudioSource bark; // Bark sfx.
        public AudioSource whine; // Whine sfx.
        public float speed = 0.1f; // Movement speed.
        public int maxHealth = 10; // Max health.
        public int currHealth; // Current health.

        bool invincible = false; // Invincibility frames.

        void Start()
        {
            currHealth = maxHealth;
        }

        void Update()
        {
            float hdir = Input.GetAxisRaw("Horizontal");
            float vdir = Input.GetAxisRaw("Vertical");

            transform.position += Vector3.right * hdir * speed;
            transform.position += Vector3.up * vdir * speed;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                bark.Play();
                var go = Instantiate(bulletPrefab, transform.parent);
                go.transform.position = transform.position;
            }
        }

        public void OnTriggerEnter2D(Collider2D col)
        {
            if (!invincible && 
               (col.GetComponent<EnemyBullet>() != null || 
                col.GetComponent<Enemy>() != null))
            {
                currHealth--;
                if (col.GetComponent<EnemyBullet>() != null)
                    Destroy(col.gameObject);
                if (currHealth <= 0)
                {
                    Debug.Log("DEAD");
                }
                else
                {
                    whine.Play();
                    invincible = true;
                    StartCoroutine(IFrames());
                }
            }
        }

        // Invincibility frame timer.
        IEnumerator IFrames()
        {
            float time = 0f;
            bool flip = false;
            while (time < 1.0f)
            {
                if (flip = !flip) sr.color = new Color(1f,0.6f,0.6f,0.5f);
                else sr.color = new Color(1f, 0.6f, 0.6f, 1f);
                yield return new WaitForSeconds(0.15f);
                time += 0.15f;
            }
            sr.color = new Color(1f, 1f, 1f, 1f);
            invincible = false;
        }
    }
}
