using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PopupBase : MonoBehaviour
{
    public abstract Coroutine PopText(string text, Vector2 pos, float time);
    public abstract Coroutine PopImage(Sprite image, Vector2 pos, float time);

    public void Cleanup()
    {
        Destroy(gameObject);
    }

    public Coroutine PopImageAndCleanup(Sprite image, Vector2 pos, float time)
    {
        return StartCoroutine(PopImageAndCleanupCR(image, pos, time));
    }

    private IEnumerator PopImageAndCleanupCR(Sprite image, Vector2 pos, float time)
    {
        yield return PopImage(image, pos, time);
        Cleanup();
    }

    public Coroutine PopTextAndCleanup(string text, Vector2 pos, float time)
    {
        return StartCoroutine(PopTextAndCleanupCR(text, pos, time));
    }

    private IEnumerator PopTextAndCleanupCR(string text, Vector2 pos, float time)
    {
        yield return PopText(text, pos, time);
        Cleanup();
    }

}
