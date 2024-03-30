using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private MenuButton firstButton;
    [SerializeField] private TweenInfo tweenInfo;
    public AudioClip GameOverAudioClip = default;

    public void Awake()
    {
        BattleManager.instance.OnGameOver.AddListener(()=>gameObject.SetActive(true));
        gameObject.SetActive(false);
    }

    public void OnEnable()
    {
        const float screenDelay = 4f;
        const float slowdownDuration = 5f;
        const float startingTimeScale = 0.1f;

        BattleManager.instance.PH.Pause = true;
        DOTween.defaultTimeScaleIndependent = true;

        //screen fade in
        var canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.DOFade(1, tweenInfo.Time).From(0).SetDelay(screenDelay)
            .OnPlay(() => AudioManager.instance.PlayBGM(GameOverAudioClip))
            .OnComplete(() => {
                if (firstButton) firstButton.InitializeSelection();
            });

        //time slowdown effect
        DOTween.To(()=>Time.timeScale, v=>Time.timeScale=v, 1f, slowdownDuration).From(startingTimeScale).SetEase(Ease.OutCubic);
        DOTween.defaultTimeScaleIndependent = false;
    }

    public void RetryBattle()
    {
        StartCoroutine(RetryBattleCR());
    }

    public IEnumerator RetryBattleCR()
    {
        const float arbitraryWaitTime = 1f; //artificial load time
        yield return new WaitForSeconds(arbitraryWaitTime);
        BattleManager.instance.Reload();
        gameObject.SetActive(false);
    }

    public void ReturnToTitle()
    {
        UnityEngine.EventSystems.EventSystem.current.enabled = false;
        TransitionManager.instance.TransitionToMainMenu();
    }
}
