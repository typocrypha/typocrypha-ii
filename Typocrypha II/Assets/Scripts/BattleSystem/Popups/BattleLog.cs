using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BattleLog : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Image icon;

    [SerializeField]
    private GameObject iconContainer;
    [SerializeField]
    private Transform logContainer;
    [SerializeField]
    private CanvasGroup logGroup;
    [SerializeField]
    private TweenInfo enterExitTweener;

    public void SetContent(string text, Sprite icon)
    {
        this.text.text = "> " + text;
        this.icon.sprite = icon;
        iconContainer.SetActive(icon != null);
    }

    public IEnumerator Play()
    {
        logContainer.transform.localScale = new Vector3(1, 0);
        logGroup.alpha = 0;
        enterExitTweener.Start(logContainer.DOScaleY(1, enterExitTweener.Time));
        enterExitTweener.Start(logGroup.DOFade(1, enterExitTweener.Time), false);
        yield return enterExitTweener.WaitForCompletion();
        yield return new WaitForSeconds(0.6f / Settings.UISpeed);
        enterExitTweener.Start(logContainer.DOScaleY(0, enterExitTweener.Time));
        enterExitTweener.Start(logGroup.DOFade(0, enterExitTweener.Time),false);
        yield return enterExitTweener.WaitForCompletion();
    }
}
