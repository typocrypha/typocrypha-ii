using System.Collections;
using System.Collections.Generic;
using Typocrypha;
using UnityEngine;
using UnityEngine.UI;

public abstract class InteractivePopup : CastBar
{
    public bool LastPromptSuccess { get; protected set; } = false;
    public bool Completed { get; protected set; } = false;

    private bool wasPaused = false;

    [SerializeField] private Image timerBar; 

    public virtual Coroutine Show(string header, string data, float time)
    {
        Setup(header, data, time);
        return StartCoroutine(PromptCr(time));
    }

    protected abstract void Setup(string header, string data, float time);

    protected virtual IEnumerator PromptCr(float time)
    {
        float currTime = 0;
        if(timerBar != null)
        {
            timerBar.fillAmount = 1;
        }
        while (!Completed)
        {
            yield return new WaitForEndOfFrame();
            if (time > 0)
            {
                currTime += Time.deltaTime * Settings.GameplaySpeed;
                if(timerBar != null)
                {
                    timerBar.fillAmount = Mathf.Max(0, 1 - (currTime / time));
                }
                if (currTime >= time)
                {
                    InputManager.Instance.CompleteInput();
                    break;
                }
            }
        }
        gameObject.SetActive(false);
    }

    public override void Focus()
    {
        base.Focus();
        wasPaused = Keyboard.instance.PH.Pause;
        if (wasPaused)
        {
            Keyboard.instance.PH.Pause = false;
        }
    }

    public override void Unfocus()
    {
        base.Unfocus();
        if (wasPaused)
        {
            wasPaused = false;
            Keyboard.instance.PH.Pause = true;
        }
    }
}
