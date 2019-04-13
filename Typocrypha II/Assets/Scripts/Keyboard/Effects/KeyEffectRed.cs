using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Typocrypha
{
    public class KeyEffectRed : KeyEffect
    {
        public override void OnStart()
        {
            StartCoroutine(DestroyAfterTime(4f));
        }

        public override void OnPress()
        {
            transform.Rotate(Vector3.forward, 30f);
        }

        public override void Remove()
        {
            key.onPress -= OnPress;
            Destroy(gameObject);
        }

        // Remove effect when time runs out.
        IEnumerator DestroyAfterTime(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            Remove();
        }
    }
}
