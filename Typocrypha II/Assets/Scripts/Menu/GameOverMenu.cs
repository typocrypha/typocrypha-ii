using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private MenuButton firstButton;
    [SerializeField] AudioClip GameOverAudioClip = default;
    [SerializeField] CanvasGroup backgroundGroup;
    [SerializeField] CanvasGroup foregroundGroup;
    [SerializeField] AnimationCurve backgroundFadeCurve;
    [SerializeField] MaterialController blurController;

    private Tween slowTween, backgroundTween, foregroundTween;
    private Sequence gameoverSequence;

    public void Start()
    {
        var defaultAutoPlay = DOTween.defaultAutoPlay;
        DOTween.defaultAutoPlay = AutoPlay.None;
        DOTween.defaultTimeScaleIndependent = true;

        //time slowdown effect
        const float slowdownDuration = 5f;
        const float initialTimeScale = 0.1f;
        slowTween = DOTween.To(() => Time.timeScale, v => Time.timeScale = v, 1f, slowdownDuration).From(initialTimeScale).SetEase(Ease.OutCubic);

        //background fade in
        const float backgroundFadeDelay = 3f;
        const float backgroundFadeDuration = 3f;
        backgroundTween = backgroundGroup.DOFade(1, backgroundFadeDuration).From(0).SetDelay(backgroundFadeDelay).SetEase(backgroundFadeCurve);

        //foreground fade in
        const float foregroundFadeDuration = 2f;
        foregroundTween = foregroundGroup.DOFade(1, foregroundFadeDuration).From(0);

        gameoverSequence = DOTween.Sequence()
            .SetAutoKill(false)
            .SetRecyclable(true)
            .AppendCallback(()=> AudioManager.instance.StopBGM(AnimationCurve.Linear(0, AudioManager.instance.BGMVolume, slowdownDuration, 0)))
            .Append(slowTween)
            .Join(blurController.GetTween())
            .Join(backgroundTween)
            .AppendInterval(0.5f)
            .AppendCallback(() => AudioManager.instance.PlayBGM(GameOverAudioClip))
            .Append(foregroundTween)
            .OnComplete(() => {
                if (firstButton) firstButton.InitializeSelection();
            });

        DOTween.defaultAutoPlay = defaultAutoPlay;
        DOTween.defaultTimeScaleIndependent = false;

        BattleManager.instance.OnGameOver.AddListener(PlayGameOverSequence);
    }

    private void PlayGameOverSequence()
    {
        BattleManager.instance.PH.Pause(PauseSources.GameOver);
        Battlefield.instance.PH.Pause(PauseSources.GameOver);
        gameoverSequence.Restart();
    }

    public void RetryBattle()
    {
        StartCoroutine(RetryBattleCR());
    }

    public IEnumerator RetryBattleCR()
    {
        EventSystem.current.SetSelectedGameObject(null);
        AudioManager.instance.StopBGM();
        const float arbitraryWaitTime = 1.0f; //artificial load time
        yield return new WaitForSeconds(arbitraryWaitTime);
        BattleManager.instance.Reload();
        BattleManager.instance.PH.Unpause(PauseSources.GameOver);
        Battlefield.instance.PH.Unpause(PauseSources.GameOver);
        foregroundGroup.alpha = backgroundGroup.alpha = 0;
    }

    public void ReturnToTitle()
    {
        EventSystem.current.SetSelectedGameObject(null);
        AudioManager.instance.StopBGM();
        TransitionManager.instance.TransitionToMainMenu();
    }
}
