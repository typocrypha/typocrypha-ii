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
            continuous,
            boss
        }

        public GameObject bulletPrefab; // Enemy projectile prefab.
        public Animator animator; // Enemy animator.
        public AudioSource takeDamage; // SFX when getting hit.
        public AudioSource deathExplosion; // Death explosion sfx.
        public AudioSource oraoraora; // Oraoraora sfx.
        public AudioSource yareyaredaze; // Yareyaredaze sfx.
        public AudioSource bgm; // BGM player.
        public AudioClip jojobgm; // Jojo bgm.
        public GameObject gogogo; // Menacing effect.
        public GameObject jotaroHat; // Jotaro's hat.
        public FiringPattern firingPattern; // Enemy firing pattern.
        public float speed = 0.1f; // Horizontal movement speed.
        public int maxHealth = 3; // Maximum health.

        int currHealth; // Current health.
        bool started = false; // Has enemy entered screen?

        protected void Start()
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
                    if (firingPattern == FiringPattern.boss)
                    {
                        yareyaredaze.Play();
                        bgm.Stop();
                    }
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
                case FiringPattern.boss:
                    StartCoroutine(Boss());
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

        // Boss pattern (stop moving once entered, and shoot a lot of bulelts).
        IEnumerator Boss()
        {
            yield return new WaitForSeconds(3f);
            speed = 0; // Stop moving.
            Coroutine rs = StartCoroutine(RandomSpray());
            Coroutine bw = StartCoroutine(BulletWall());
            while (currHealth > 200) // Phase 1.
            {
                yield return new WaitForEndOfFrame();
            }
            // Phase 2.
            bgm.clip = jojobgm;
            StopCoroutine(rs);
            StopCoroutine(bw);
            yield return new WaitForSeconds(0.5f);
            yareyaredaze.Play();
            var go = Instantiate(gogogo, transform);
            go.transform.localScale = Vector3.one * go.transform.localScale.x * 2;
            foreach(Transform tr in go.transform)
            {
                tr.GetComponent<SpriteRenderer>().sortingLayerName = "TIPSEntry";
                tr.GetComponent<SpriteRenderer>().sortingOrder = 45;
            }
            go = Instantiate(jotaroHat, transform);
            go.transform.localScale = Vector3.one * go.transform.localScale.x * 0.3f;
            go.transform.Translate(4f, -2f, 0);
            yield return new WaitForSeconds(2f);
            
            bgm.Play();

            StartCoroutine(BulletWall());
            StartCoroutine(Oraoraora());
            StartCoroutine(Spray());
        }

        // Spray bullets randomly in cone.
        IEnumerator RandomSpray()
        {
            while (true)
            {
                yield return new WaitForSeconds(2f);
                for (int i = 0; i < 10; i++)
                {
                    yield return new WaitForSeconds(0.1f);
                    float rnd = Random.value * (Mathf.PI / 3f);
                    rnd -= Mathf.PI / 6f;
                    var go = Instantiate(bulletPrefab, transform.parent.parent);
                    go.transform.position = transform.position;
                    Debug.Log(-Mathf.Cos(rnd) + ":" + Mathf.Sin(rnd));
                    go.GetComponent<EnemyBullet>().xspeed = Mathf.Cos(rnd) * 0.1f;
                    go.GetComponent<EnemyBullet>().yspeed = Mathf.Sin(rnd) * 0.1f;
                }
            }
        }

        // Fire a wall of bullets on half the screen.
        IEnumerator BulletWall()
        {
            while (true)
            {
                yield return new WaitForSeconds(4f);
                float rnd = Random.value;
                for (int i = 0; i < 10; i++)
                {
                    var go = Instantiate(bulletPrefab, transform.parent.parent);
                    go.transform.position = transform.position;
                    if (rnd > 0.5f)
                        go.transform.Translate(0f, i * 0.3f, 0f);
                    else
                        go.transform.Translate(0f, -i * 0.3f, 0f);
                }
            }
        }

        // Fire a bunch of bullets, but it's Jojo now.
        IEnumerator Oraoraora()
        {
            while (true)
            {
                yield return new WaitForSeconds(6f);
                oraoraora.Play();
                for (int i = 0; i < 20; i++)
                {
                    yield return new WaitForSeconds(0.1f);
                    float rnd = Random.value * (Mathf.PI / 3f);
                    rnd -= Mathf.PI / 6f;
                    var go = Instantiate(bulletPrefab, transform.parent.parent);
                    go.transform.position = transform.position;
                    Debug.Log(-Mathf.Cos(rnd) + ":" + Mathf.Sin(rnd));
                    go.GetComponent<EnemyBullet>().xspeed = Mathf.Cos(rnd) * 0.2f;
                    go.GetComponent<EnemyBullet>().yspeed = Mathf.Sin(rnd) * 0.2f;
                    go.GetComponentInChildren<SpriteRenderer>().color = Color.blue;
                }
            }
        }

        // Fires several bullets at once.
        IEnumerator Spray()
        {
            while (true)
            {
                yield return new WaitForSeconds(3f);
                for (int i = 0; i < 6; i++)
                {
                    float rnd = Random.value * (Mathf.PI / 3f);
                    rnd -= Mathf.PI / 6f;
                    var go = Instantiate(bulletPrefab, transform.parent.parent);
                    go.transform.position = transform.position;
                    Debug.Log(-Mathf.Cos(rnd) + ":" + Mathf.Sin(rnd));
                    go.GetComponent<EnemyBullet>().xspeed = Mathf.Cos(rnd) * 0.1f;
                    go.GetComponent<EnemyBullet>().yspeed = Mathf.Sin(rnd) * 0.1f;
                }
            }
        }
    }
}

