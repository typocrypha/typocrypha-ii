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

    [SerializeField] private FilledSlicedImage timerBar; 

    public virtual Coroutine Show(string header, string data, float time)
    {
        Setup(header, data, time);
        float modTime = time;
        if(PlayerDataManager.Equipment.TryGetEquippedBadgeEffect<BadgeEffectExtendPromptTimer>(out var promptExtendedEffect))
        {
            modTime *= promptExtendedEffect.Multiplier;
        }
        return StartCoroutine(PromptCr(modTime));
    }

    protected abstract void Setup(string header, string data, float time);

    protected virtual IEnumerator PromptCr(float time)
    {
        float currTime = 0;
        if(timerBar != null)
        {
            timerBar.FillAmount = 1;
        }
        while (!Completed)
        {
            yield return new WaitForEndOfFrame();
            if (time > 0)
            {
                currTime += Time.deltaTime * Settings.GameplaySpeed;
                if(timerBar != null)
                {
                    timerBar.FillAmount = Mathf.Max(0, 1 - (currTime / time));
                }
                if (currTime >= time)
                {
                    OnTimeout();
                    InputManager.Instance.CompleteInput();
                    break;
                }
            }
        }
        gameObject.SetActive(false);
    }

    protected virtual void OnTimeout()
    {

    }
}
