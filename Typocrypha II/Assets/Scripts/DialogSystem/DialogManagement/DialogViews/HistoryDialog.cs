using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HistoryDialog : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private readonly DialogItemHistory dialogItem = new DialogItemHistory("", new List<AudioClip>());

    public void SetData(DialogHistory.HistoryData data)
    {
        Cleanup();
        // Setup dialog item
        dialogItem.text = $"{data.Speaker}: {data.Text}";
        // Parse text
        DialogParser.instance.Parse(dialogItem, gameObject, text, false);
        // Set text
        text.text = dialogItem.text;
    }

    public void Cleanup()
    {
        // Remove old text effects.
        FXText.TMProEffect.Cleanup(gameObject);
        text.text = string.Empty;
    }
}
