using System;
using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class VictoryResultsScreen : MonoBehaviour
{
    [System.Serializable]
    public struct TallyEntry
    {
        public string label; //value description
        public int value; //contributes to the total
        public bool isPercentage; //whether value affects base total or final multiplier
    }

    [Header("Initialization")]
    [SerializeField] private MenuButton firstButton;
    [SerializeField] private TweenInfo tweenInfo;
    [SerializeField] private GameObject clarkeContainer;
    [SerializeField] private GameObject messageContainer;
    [SerializeField] private CharacterData clarkeData;

    [Header("Display Text")]
    [SerializeField] private TMPro.TextMeshProUGUI tally;
    [SerializeField] private TMPro.TextMeshProUGUI total;
    [SerializeField] private TMPro.TextMeshProUGUI balance;
    [SerializeField] private TMPro.TextMeshProUGUI message;

    [Header("Clarke Tweens")]
    [SerializeField] private Vector3 initialPosition;
    [SerializeField] private Vector3 finalPosition;
    [SerializeField] private AnimationCurve clarkeSlideVertical = default;
    [SerializeField] private AnimationCurve clarkeSlideHorizontal = default;

    public Action OnContinuePressed;

    const int LINE_LENGTH = 36;

    //animation values
    const float TALLY_DELAY_AFTER = 0.75f;
    const float TOTAL_DELAY_AFTER = 1.50f;

    const string FORMAT_DOLLAR = "+$#;-$#;$0";
    const string FORMAT_PERCENT = @"+#\%;-#\%;0\%";
    const string FORMAT_BALANCE = "$#;-$#;$0";

    public void Awake()
    {
        gameObject.SetActive(false);
    }

    public void DisplayResults(TallyEntry[] tallies, int total, int balance, string clarkeText)
    {
        gameObject.SetActive(true);
        messageContainer.gameObject.SetActive(false);
        clarkeContainer.GetComponent<RectTransform>().anchoredPosition = initialPosition;
        StartCoroutine(DisplayResultsCR(tallies, total, balance, clarkeText));
    }

    IEnumerator DisplayResultsCR(TallyEntry[] tallies, int total, int balance, string clarkeText)
    {
        ClearAllText();
        firstButton.gameObject.SetActive(false);
        tweenInfo.Start(GetComponent<CanvasGroup>().DOFade(1, tweenInfo.Time).From(0));
        yield return tweenInfo.WaitForCompletion();
        yield return DisplayClarke();
        yield return new WaitForSeconds(1f);
        foreach (var entry in tallies) yield return DisplayTally(entry);
        yield return new WaitForSeconds(1f);
        yield return DisplayTotal(total);
        yield return DisplayBalance(balance, balance+total);
        //activate button
        if (firstButton)
        {
            var unityButton = firstButton.GetComponent<UnityEngine.UI.Button>();
            unityButton.interactable = true;
            unityButton.onClick.AddListener(new UnityAction(OnContinuePressed));
            firstButton.gameObject.SetActive(true);
            firstButton.InitializeSelection();
        }
        yield return DisplayMessage(clarkeText);
    }

    private void ClearAllText()
    {
        tally.text = total.text = balance.text = message.text = "";
    }

    private IEnumerator DisplayTally(TallyEntry entry)
    {
        tally.text += entry.label;
        string color = entry.value > 0 ? "green" : entry.value < 0 ? "red" : "white";
        string format = entry.isPercentage ? FORMAT_PERCENT : FORMAT_DOLLAR;
        var padAmount = LINE_LENGTH - entry.label.Length;
        tally.text += $"<color={color}>{entry.value.ToString(format).PadLeft(padAmount)}</color>\n";
        yield return new WaitForSeconds(TALLY_DELAY_AFTER);
    }

    private IEnumerator DisplayTotal(int value)
    {
        var label = "Total";
        var padAmount = LINE_LENGTH - label.Length;
        total.text = label + $"{value.ToString(FORMAT_DOLLAR).PadLeft(padAmount)}\n";
        yield return new WaitForSeconds(TOTAL_DELAY_AFTER);
    }

    private IEnumerator DisplayBalance(int oldBalance, int newBalance)
    {
        var label = "Balance";
        var padAmount = LINE_LENGTH - label.Length;
        balance.text = label + $"{oldBalance.ToString(FORMAT_DOLLAR).PadLeft(padAmount)}\n";
        //animation
        int runningTotal = oldBalance;
        var tween = DOTween.To(() => runningTotal, v => runningTotal = v, newBalance, 1f)
            .SetDelay(1f)
            .SetEase(Ease.InOutExpo)
            .OnUpdate( () => { balance.text = label + $"{runningTotal.ToString(FORMAT_BALANCE).PadLeft(padAmount)}\n"; } );
        yield return tween.WaitForCompletion();
        yield return new WaitForSeconds(TOTAL_DELAY_AFTER);
    }

    private IEnumerator DisplayClarke()
    {
        const float duration = 0.8f;
        var clarkeTransform = clarkeContainer.GetComponent<RectTransform>();
        clarkeTransform.DOAnchorPosX(finalPosition.x, duration).SetEase(clarkeSlideHorizontal);
        clarkeTransform.DOAnchorPosY(finalPosition.y, duration).SetEase(clarkeSlideVertical);
        yield return clarkeTransform.DOScale(new Vector2(-1,1), duration).From(new Vector2(-1,1)/2).WaitForCompletion();
    }

    private IEnumerator DisplayMessage(string clarkeText)
    {
        messageContainer.gameObject.SetActive(true);
        for (int i = 0; i <= clarkeText.Length; i++)
        {
            message.text = clarkeText.Substring(0,i);
            const float defaultScroll = 0.021f;
            const int SpeechInterval = 3;
            if (i % SpeechInterval == 0 && !char.IsWhiteSpace(clarkeText[i]))
            {
                AudioManager.instance.PlayTextScrollSfx(clarkeData.talk_sfx);
            }
            yield return new WaitForSeconds(defaultScroll / Settings.TextScrollSpeed);
        }
    }
}
