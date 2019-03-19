using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bullethell
{
    /// <summary>
    /// Enemy behaviour in 'Bullethell' TIPS minigame.
    /// Constanly move to the left.
    /// Other movements controlled by root-position-relative animations.
    /// </summary>
    public class Enemy : MonoBehaviour
    {
        public enum FiringPattern
        {
            none,
            oneshot,
            continuous
        }

        public GameObject bulletPrefab; // Enemy projectile prefab.
        public Animator animator; // Enemy animator.
        public AudioSource takeDamage; // SFX when getting hit.
        public AudioSource deathExplosion; // Death explosion sfx.
        public FiringPattern firingPattern; // Enemy firing pattern.
        public float speed = 0.1f; // Horizontal movement speed.
        public int maxHealth = 3; // Maximum health.

        int currHealth; // Current health.
        bool started = false; // Has enemy entered screen?

        void Start()
        {
            currHealth = maxHealth;
        }

        void Update()
        {
            transform.parent.Translate(-speed, 0, 0);
        }

        public void OnTriggerEnter2D(Collider2D col)
        {
            if (col.GetComponent<Bullet>() != null)
            {
                currHealth--;
                Destroy(col.gameObject);
                if (currHealth <= 0)
                {
                    deathExplosion.Play();
                    StopAllCoroutines(); // Stop firing pattern.
                    speed = 0f; // Stop enemy movement.
                    GetComponent<Collider2D>().enabled = false; // Disable collider.
                    animator.Play("Death");
                }
                else
                {
                    takeDamage.Play();
                }
            }
            else if (col.gameObject.name == "EnemyStartLine")
            {
                if (!started)
                {
                    started = true;
                    FireBullets();
                }
            }
        }

        // Fire bullets based on pattern.
        void FireBullets()
        {
            switch (firingPattern)
            {
                case FiringPattern.oneshot:
                    StartCoroutine(Oneshot());
                    break;
                case FiringPattern.continuous:
                    StartCoroutine(Continuous());
                    break;
                default:
                    break;
            }
        }

        // Oneshot firing pattern (just fires once).
        IEnumerator Oneshot()
        {
            yield return new WaitForSeconds(1.5f);
            var go = Instantiate(bulletPrefab, transform.parent.parent);
            go.transform.position = transform.position;
        }

        // Continuous firing pattern (keeps firing).
        IEnumerator Continuous()
        {
            while(true)
            {
                yield return new WaitForSeconds(1.5f);
                var go = Instantiate(bulletPrefab, transform.parent.parent);
                go.transform.position = transform.position;
            }
        }
    }
}

