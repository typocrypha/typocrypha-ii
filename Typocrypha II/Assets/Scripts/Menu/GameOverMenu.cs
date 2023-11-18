using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private MenuButton firstButton;
    [SerializeField] private TweenInfo tweenInfo;

    public void Awake()
    {
        BattleManager.instance.OnGameOver.AddListener(()=>gameObject.SetActive(true));
        gameObject.SetActive(false);
    }

    public void OnEnable()
    {
        if (firstButton) firstButton.InitializeSelection();
        tweenInfo.Start(GetComponent<CanvasGroup>().DOFade(1, tweenInfo.Time).From(0));
    }

    public void RetryBattle()
    {
        BattleManager.instance.Reload();
        gameObject.SetActive(false);
    }

    public void ReturnToTitle()
    {
        UnityEngine.EventSystems.EventSystem.current.enabled = false;
        TransitionManager.instance.TransitionToMainMenu();
    }
}
