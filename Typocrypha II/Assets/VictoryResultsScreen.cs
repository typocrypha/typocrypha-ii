using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Gameflow;

public class VictoryResultsScreen : MonoBehaviour
{
    [System.Serializable]
    public struct TallyEntry
    {
        public string label;
        public int value;
    }

    [Header("Initialization")]
    [SerializeField] private MenuButton firstButton;
    [SerializeField] private TweenInfo tweenInfo;

    [Header("Display Text")]
    [SerializeField] TMPro.TextMeshProUGUI tally;
    [SerializeField] TMPro.TextMeshProUGUI total;

    const int LINE_LENGTH = 36;

    public void Awake()
    {
        gameObject.SetActive(false);
    }

    public void OnEnable()
    {
    }

    public void DisplayResults(VictoryScreenNode results)
    {
        gameObject.SetActive(true);
        StartCoroutine(DisplayResultsCR(results));
    }

    IEnumerator DisplayResultsCR(VictoryScreenNode results)
    {
        ClearAllText();
        firstButton.GetComponent<UnityEngine.UI.Button>().interactable = false;
        tweenInfo.Start(GetComponent<CanvasGroup>().DOFade(1, tweenInfo.Time).From(0));
        yield return tweenInfo.WaitForCompletion();
        yield return new WaitForSeconds(1f);
        foreach (var entry in results.Entries)
        {
            yield return AddTally(entry.label, entry.value, 0f, 0.75f);
        }
        yield return new WaitForSeconds(1f);
        yield return DisplayTotal(results.Total, 2f);
        //activate button
        if (firstButton)
        {
            firstButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
            firstButton.InitializeSelection();
        }
    }

    void ClearAllText()
    {
        tally.text = total.text = "";
    }

    IEnumerator AddTally(string label, int value, float delayInBetween = 0f, float delayAfter = 0f)
    {
        tally.text += label;
        yield return new WaitForSeconds(delayInBetween);
        string color = value > 0 ? "green" : value < 0 ? "red" : "white";
        var padAmount = LINE_LENGTH - label.Length;
        tally.text += $"<color={color}>{value.ToString("+$#;-$#;$0").PadLeft(padAmount)}</color>\n";
        yield return new WaitForSeconds(delayAfter);
    }

    IEnumerator DisplayTotal(int value, float postDelay = 0f)
    {
        var label = "Total";
        string color = value > 0 ? "green" : value < 0 ? "red" : "white";
        var padAmount = LINE_LENGTH - label.Length;
        total.text = label + $"<color={color}>{value.ToString("+$#;-$#;$0").PadLeft(padAmount)}</color>\n";
        yield return new WaitForSeconds(postDelay);
    }

    public void OnContinuePressed()
    {
        BattleManager.instance.NextWave();
    }
}
