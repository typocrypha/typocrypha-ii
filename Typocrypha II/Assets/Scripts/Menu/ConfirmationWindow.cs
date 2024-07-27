using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utilities.Unity;

public class ConfirmationWindow : MonoBehaviour
{
    [SerializeField] MenuButton confirm;
    [SerializeField] MenuButton deny;
    [SerializeField] TMPro.TextMeshProUGUI promptUI;
    [SerializeField] CanvasGroup canvasGroup;

    public MenuButton Confirm => confirm;
    public MenuButton Deny => deny;

    private Vector3 initialAnchorPos;

    public void Awake()
    {
        initialAnchorPos = (transform as RectTransform).anchoredPosition;
    }

    public void Open(string promptText = null, bool defaultToConfirm = true)
    {
        if (promptText != null) promptUI.text = promptText;
        gameObject.SetActive(true);
        DOTween.Kill("OpenConfirmation");
        DOTween.Kill("CloseConfirmation");
        DOTween.Sequence()
            .SetId("OpenConfirmation")
            .Join((transform as RectTransform).DOScaleX(1f, 0.1f).From(0f))
            .Join(canvasGroup.DOFade(1f, 0.1f).SetEase(Ease.Linear));
        (defaultToConfirm ? Confirm : Deny).InitializeSelection();
    }

    public void Close()
    {
        DOTween.Kill("OpenConfirmation");
        DOTween.Kill("CloseConfirmation");
        DOTween.Sequence()
            .SetId("CloseConfirmation")
            .Join((transform as RectTransform).DOScaleX(0f, 0.1f).From(1f))
            .Join(canvasGroup.DOFade(0f, 0.1f).SetEase(Ease.Linear))
            .OnComplete(() => gameObject.SetActive(false));
    }

    public void SetConfirmAction(params Action[] callbacks)
    {
        Confirm.button.onClick.ReplaceAllListeners(callbacks);
        Confirm.button.onClick.AddListener(Close);
    }

    public void SetDenyAction(params Action[] callbacks)
    {
        Deny.button.onClick.ReplaceAllListeners(callbacks);
        Deny.button.onClick.AddListener(Close);
    }

}
