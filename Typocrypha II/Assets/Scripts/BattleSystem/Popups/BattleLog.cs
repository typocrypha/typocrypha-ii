using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BattleLog : MonoBehaviour
{
    public const float defaultLogTime = 0.6f;

    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image icon;
    [SerializeField]
    private GameObject iconContainer;
    [SerializeField]
    private Transform logContainer;
    [SerializeField]
    private CanvasGroup logGroup;
    [SerializeField]
    private TweenInfo enterExitTweener;

    private float time;

    public void SetContent(string text, Sprite icon, float? time = null)
    {
        this.text.text = "> " + text;
        this.icon.sprite = icon;
        this.time = time ?? defaultLogTime;
        iconContainer.SetActive(icon != null);
    }

    public IEnumerator Play()
    {
        logContainer.transform.localScale = new Vector3(1, 0);
        logGroup.alpha = 0;
        enterExitTweener.Start(logContainer.DOScaleY(1, enterExitTweener.Time));
        enterExitTweener.Start(logGroup.DOFade(1, enterExitTweener.Time), false);
        yield return enterExitTweener.WaitForCompletion();
        yield return new WaitForSeconds(time / Settings.UISpeed);
        enterExitTweener.Start(logContainer.DOScaleY(0, enterExitTweener.Time));
        enterExitTweener.Start(logGroup.DOFade(0, enterExitTweener.Time),false);
        yield return enterExitTweener.WaitForCompletion();
    }
}
