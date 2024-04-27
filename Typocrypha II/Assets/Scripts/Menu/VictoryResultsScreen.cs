using System;
using System.Collections;
using System.Diagnostics;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class VictoryResultsScreen : MonoBehaviour
{
    [System.Serializable]
    public struct TallyEntry
    {
        public string label; //value description
        public int value; //contributes to the total
        public bool isPercentage; //whether value affects base total or final multiplier

        public TallyEntry(string label, int value, bool isPercentage)
        {
            this.label = label;
            this.value = value;
            this.isPercentage = isPercentage;
        }
    }
    public struct BonusEntry
    {
        public string badgeName; //badge name
        public string unlockReason; //how the badge was earned
        public string description; //describes the effects of wearing the badge

        public BonusEntry(string badgeName, string unlockReason, string description)
        {
            this.badgeName = badgeName;
            this.unlockReason = unlockReason;
            this.description = description;
        }
    }

    [Header("Initialization")]
    [SerializeField] private Button continueButton;
    [SerializeField] private TweenInfo tweenInfo;
    [SerializeField] private GameObject clarkeContainer;
    [SerializeField] private GameObject messageContainer;
    [SerializeField] private CharacterData clarkeData;
    [SerializeField] private RectTransform tallyView;
    [SerializeField] private VerticalLayoutGroup bonusView;

    [Header("Display Text")]
    [SerializeField] private TextMeshProUGUI header;
    [SerializeField] private TextMeshProUGUI tally;
    [SerializeField] private TextMeshProUGUI total;
    [SerializeField] private TextMeshProUGUI balance;
    [SerializeField] private TextMeshProUGUI message;

    [Header("Clarke Tweens")]
    [SerializeField] private Vector3 initialPosition;
    [SerializeField] private Vector3 finalPosition;
    [SerializeField] private AnimationCurve clarkeSlideVertical = default;
    [SerializeField] private AnimationCurve clarkeSlideHorizontal = default;

    public Action OnScreenClosed;

    const int LINE_LENGTH = 36;

    //animation values
    const float TALLY_DELAY_AFTER = 0.75f;
    const float TOTAL_DELAY_AFTER = 1.50f;
    const float MASK_HEIGHT = 520f;

    const string FORMAT_DOLLAR = "+$#;-$#;$0";
    const string FORMAT_PERCENT = @"+#\%;-#\%;0\%";
    const string FORMAT_BALANCE = "$#;-$#;$0";

    private bool shouldShowBonuses = false;

    private Sequence bonusScroll;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    [Conditional("DEBUG"), ContextMenu("Test")]
    public void TestDisplay()
    {
        var tallies = new TallyEntry[] {
            new TallyEntry("Tally A", 200, false),
            new TallyEntry("Tally B", 100, false),
            new TallyEntry("Tally C", -10, true),
        };
        DisplayResults(tallies, 270, 1000, "goobaba!");
        SetBonuses(new BonusEntry[] {
            new BonusEntry("Badge A", "Reason A", "This badge does a lot of good things when you equip it."),
            new BonusEntry("Badge B", "Reason B", "This badge also does a lot of good things when you equip it."),
            new BonusEntry("Badge C", "Reason C", "This badge is either really good or really bad, idk."),
        });
    }

    [Conditional("DEBUG"), ContextMenu("Test2")]
    public void TestDisplay2()
    {
        var tallies = new TallyEntry[] {
            new TallyEntry("Tally A", 200, false),
            new TallyEntry("Tally B", 100, false),
            new TallyEntry("Tally C", -10, true),
        };
        DisplayResults(tallies, 270, 1000, "goobaba!");
        SetBonuses(new BonusEntry[] {
            new BonusEntry("Badge A", "Reason A", "This badge does a lot of good things when you equip it."),
            new BonusEntry("Badge B", "Reason B", "This badge also does a lot of good things when you equip it."),
            new BonusEntry("Badge C", "Reason C", "This badge is either really good or really bad, idk."),
            new BonusEntry("Badge D", "Reason D", "This badge has a slight chance to make the user explode."),
            new BonusEntry("Badge E", "Reason E", "This badge will make your wildest dreams come true."),
            new BonusEntry("Badge F", "Reason F", "This badge is for aesthetic purposes only."),
            //new BonusEntry("Badge G", "Reason F", "This badge is for aesthetic purposes only."),
            //new BonusEntry("Badge H", "Reason F", "This badge is for aesthetic purposes only."),
            //new BonusEntry("Badge I", "Reason F", "This badge is for aesthetic purposes only."),
            //new BonusEntry("Badge J", "Reason F", "This badge is for aesthetic purposes only."),
        });
    }

    public void DisplayResults(TallyEntry[] tallies, int total, int balance, string clarkeText)
    {
        (tallyView.transform as RectTransform).anchoredPosition = new Vector3(0f, 0f);
        (bonusView.transform as RectTransform).anchoredPosition = new Vector3(1000f, 0f);
        ClearAllText();
        DisableButton();
        gameObject.SetActive(true);
        messageContainer.gameObject.SetActive(false);
        clarkeContainer.GetComponent<RectTransform>().anchoredPosition = initialPosition;
        StartCoroutine(DisplayResultsCR(tallies, total, balance, clarkeText));
    }

    IEnumerator DisplayResultsCR(TallyEntry[] tallies, int total, int balance, string clarkeText)
    {
        tweenInfo.Start(GetComponent<CanvasGroup>().DOFade(1, tweenInfo.Time).From(0));
        yield return tweenInfo.WaitForCompletion();
        yield return DisplayClarke();
        yield return new WaitForSeconds(1f);
        foreach (var entry in tallies) yield return DisplayTally(entry);
        yield return new WaitForSeconds(1f);
        yield return DisplayTotal(total);
        yield return DisplayBalance(balance, balance+total);

        yield return DisplayMessage(clarkeText);

        yield return new WaitForSeconds(1f);
        //activate button
        if (shouldShowBonuses) SetButtonAction(TransitionToBonusScreen);
        else SetButtonToExit();
    }

    public void SetBonuses(BonusEntry[] bonuses)
    {
        foreach (Transform child in bonusView.transform) child.gameObject.SetActive(false);
        for (int i = 0; i < bonuses.Length; i++)
        {
            if (i >= bonusView.transform.childCount)
            {
                Instantiate(bonusView.transform.GetChild(1).gameObject, bonusView.transform);
            }
            var entryGO = bonusView.transform.GetChild(i).gameObject;
            entryGO.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = bonuses[i].badgeName;
            entryGO.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = bonuses[i].unlockReason;
            entryGO.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = bonuses[i].description;
            entryGO.SetActive(true);
        }
        shouldShowBonuses = true;
    }

    public void TransitionToBonusScreen()
    {
        header.text = "Bonus";
        HideMessage();

        var tallyRectTransform = tallyView.transform as RectTransform;
        var bonusRectTransform = bonusView.transform as RectTransform;
        tallyRectTransform.anchoredPosition = new Vector3(0f, 0f);
        bonusRectTransform.anchoredPosition = new Vector3(1000f, 0f);
        tallyRectTransform.DOAnchorPosX(-1000f, 0.5f).SetRelative();
        bonusRectTransform.DOAnchorPosX(-1000f, 0.5f).SetRelative();

        var scrollHeight = Mathf.Max(0, (bonusView.transform as RectTransform).rect.height) - MASK_HEIGHT;
        if (scrollHeight > 0)
        {
            const float secondsPerBadge = 0.75f;
            var waitTime = 3f + Mathf.Max((bonusView.transform.childCount-8) * secondsPerBadge, 0f);
            bonusScroll = DOTween.Sequence()
                .SetDelay(secondsPerBadge * 4 * 0.5f, false) //this time is only waited on entry
                .AppendInterval(secondsPerBadge * 4 * 0.5f) //this time is waited at the start and end of each loop
                .Append(bonusView.transform.DOBlendableLocalMoveBy(Vector3.up * scrollHeight, waitTime).SetEase(Ease.Linear))
                .AppendInterval(secondsPerBadge * 0.5f)
                .AppendCallback(SetButtonToExit)
                .AppendInterval(secondsPerBadge * 0.5f)
                .SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            DOTween.Sequence().AppendInterval(4f).AppendCallback(SetButtonToExit);
        }
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

    private void HideMessage()
    {
        messageContainer.gameObject.SetActive(false);
    }

    private void DisableButton()
    {
        continueButton.interactable = false;
        continueButton.gameObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void SetButtonAction(params Action[] callbacks)
    {
        continueButton.interactable = true;
        continueButton.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(continueButton.gameObject);
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(DisableButton);
        foreach (var callback in callbacks)
        {
            if (callback != null)
            continueButton.onClick.AddListener(new UnityAction(callback));
        }
    }

    private void SetButtonToExit()
    {
        SetButtonAction(OnScreenClosed, KillBonusScroll);
    }

    private void KillBonusScroll()
    {
        if (bonusScroll.IsActive()) bonusScroll.Kill();
    }
}
