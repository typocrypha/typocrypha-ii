using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TIPSEntryPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI content;

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
}
