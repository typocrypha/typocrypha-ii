using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TIPSEntryPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI content;
    [SerializeField] TextMeshProUGUI footer;
    [SerializeField] TextMeshProUGUI page;
    [SerializeField] DialogContinueIndicator paginationIndicator;

    private readonly List<MonoBehaviour> dummy = new List<MonoBehaviour>();

    public void SetTitle(string text)
    {
        title.text = text;
    }

    public void SetContent(string text)
    {
        FXText.TMProEffect.Cleanup(dummy);
        content.text = DialogParser.instance.Parse
            (
                line: text,
                fxContainer: gameObject,
                textUI:content,
                textEvents:null,
                textEffects: dummy,
                tipsEntries: out var _,
                createEvents:false
            );
    }

    public void SetFooter(string text)
    {
        footer.text = text;
    }

    public void ShowPaginationIndicator()
    {
        paginationIndicator.Activate();
    }

    public void HidePaginationIndicator()
    {
        paginationIndicator.StopAnimation();
    }

    public void ReverseIndicator(bool onLastPage)
    {
        paginationIndicator.transform.localScale = new Vector2 (1, onLastPage ? -1 : 1);
    }

    public void DisplayPageNum(int cur, int max) => page.text = $"{cur}/{max}";
}
