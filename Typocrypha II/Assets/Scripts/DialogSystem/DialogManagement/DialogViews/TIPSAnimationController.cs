//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TIPSAnimationController : MonoBehaviour
{
    [Header("External References")]
    [SerializeField] RectTransform rectChat;
    [Header("Internal References")]
    [SerializeField] RectTransform rectTIPS;
    [SerializeField] GameObject canvasTIPS;
    [SerializeField] TIPSNavigator navigator;

    [Header("Settings")]
    [SerializeField] float tweenDuration = 0.5f;

    private bool isOpen = false;
    private Sequence sequenceOpenClose;

    private void Awake()
    {
        canvasTIPS.SetActive(false);
        navigator.OnExit += CloseTIPS;
    }

    private void OnDestroy()
    {
        navigator.OnExit -= CloseTIPS;
    }

    private void Update()
    {
        if (TIPSManager.Instance.PH.IsPaused()) return;
        if (sequenceOpenClose != null && sequenceOpenClose.IsActive() && sequenceOpenClose.IsPlaying()) return;
        if (!Input.GetKeyDown(KeyCode.Tab)) return;

        if (isOpen)
        {
            CloseTIPS();
        }
        else if (DialogManager.instance?.ReadyToContinue ?? false)
        {
            OpenTIPS();
        }
    }

    public void OpenTIPS()
    {
        isOpen = true;
        TIPSManager.Instance.PauseAllForTIPS(true);
        SequenceOpenTIPS().Play();
    }

    private Sequence SequenceOpenTIPS()
    {
        var defaultEase = DOTween.defaultEaseType;
        DOTween.defaultEaseType = Ease.InOutExpo;

        DOTween.Complete("TIPS");
        sequenceOpenClose = DOTween.Sequence().SetId("TIPS").
            Append(rectChat.DOScaleX(0, tweenDuration).From(1)).
            AppendCallback(ShowTIPSCanvas).
            Append(rectTIPS.DOScaleX(1, tweenDuration).From(0));

        DOTween.defaultEaseType = defaultEase;
        return sequenceOpenClose;
    }

    private void ShowTIPSCanvas()
    {
        canvasTIPS.SetActive(true);
        navigator.enabled = true;
        navigator.InitializeView();
    }

    public void CloseTIPS()
    {
        isOpen = false;
        navigator.enabled = false;
        SequenceCloseTIPS().OnComplete(OnCloseComplete);
    }

    private Sequence SequenceCloseTIPS()
    {
        var defaultEase = DOTween.defaultEaseType;
        DOTween.defaultEaseType = Ease.InOutExpo;

        DOTween.Complete("TIPS");
        sequenceOpenClose = DOTween.Sequence().SetId("TIPS").
            Append(rectTIPS.DOScaleX(0, tweenDuration).From(1)).
            AppendCallback(() => canvasTIPS.SetActive(false)).
            Append(rectChat.DOScaleX(1, tweenDuration).From(0));

        DOTween.defaultEaseType = defaultEase;
        return sequenceOpenClose;
    }

    private void OnCloseComplete()
    {
        TIPSManager.Instance.PauseAllForTIPS(false);
    }
}
