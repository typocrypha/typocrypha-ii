using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractivePopup : MonoBehaviour, IInputHandler
{
    public bool LastPromptSuccess { get; protected set; } = false;
    public bool Completed { get; protected set; } = false;

    public abstract void Focus();
    public abstract void Unfocus();
    public virtual Coroutine Show(string header, string data, float time)
    {
        Setup(header, data, time);
        return StartCoroutine(PromptCr(time));
    }

    protected abstract void Setup(string header, string data, float time);

    protected virtual IEnumerator PromptCr(float time)
    {
        float currTime = 0;
        while (!Completed)
        {
            yield return new WaitForEndOfFrame();
            if (time > 0)
            {
                currTime += Time.deltaTime * Settings.GameplaySpeed;
                if (currTime >= time)
                    break;
            }
        }
        gameObject.SetActive(false);
    }
}
