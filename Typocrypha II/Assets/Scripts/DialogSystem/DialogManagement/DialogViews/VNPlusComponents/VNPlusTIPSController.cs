//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class VNPlusTIPSController : MonoBehaviour
{
    public PauseHandle PH { get; private set; } = null;

    [Header("References")]
    [SerializeField] RectTransform rectChat, rectTIPS;
    [SerializeField] TIPSCastBar searchbar;

    [Header("Settings")]
    [SerializeField] float tweenDuration = 0.5f;

    private bool isOpen = false;
    private Sequence sequenceOpenClose;

    void Update()
    {
        if (sequenceOpenClose == null || !sequenceOpenClose.IsPlaying())
        {
            var mgr = DialogManager.instance;
            if (!isOpen && mgr && mgr.ReadyToContinue && Input.GetKeyDown(KeyCode.Tab))
            {
                isOpen = true;
                PauseManager.instance.PauseAll(true, PauseSources.TIPS, PH, true);
                OpenTIPS().OnComplete(OnOpenComplete);
            }
            else if (isOpen && (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape)))
            {
                isOpen = false;
                searchbar.enabled = false;
                CloseTIPS().OnComplete(OnCloseComplete);
            }
        }
    }

    private Sequence OpenTIPS()
    {
        var defaultEase = DOTween.defaultEaseType;
        DOTween.defaultEaseType = Ease.InOutExpo;

        DOTween.Complete("TIPS");
        sequenceOpenClose = DOTween.Sequence().SetId("TIPS").
            Append(rectChat.DOScaleX(0, tweenDuration).From(1)).
            AppendCallback(()=>rectTIPS.gameObject.SetActive(true)).
            Append(rectTIPS.DOScaleX(1, tweenDuration).From(0));

        DOTween.defaultEaseType = defaultEase;
        return sequenceOpenClose;
    }

    private void OnOpenComplete()
    {
        searchbar.enabled = true;
    }

    private Sequence CloseTIPS()
    {
        var defaultEase = DOTween.defaultEaseType;
        DOTween.defaultEaseType = Ease.InOutExpo;

        DOTween.Complete("TIPS");
        sequenceOpenClose = DOTween.Sequence().SetId("TIPS").
            Append(rectTIPS.DOScaleX(0, tweenDuration).From(1)).
            AppendCallback(() => rectTIPS.gameObject.SetActive(false)).
            Append(rectChat.DOScaleX(1, tweenDuration).From(0));

        DOTween.defaultEaseType = defaultEase;
        return sequenceOpenClose;
    }

    private void OnCloseComplete()
    {
        PauseManager.instance.PauseAll(false, PauseSources.TIPS, PH, true);
    }
}
