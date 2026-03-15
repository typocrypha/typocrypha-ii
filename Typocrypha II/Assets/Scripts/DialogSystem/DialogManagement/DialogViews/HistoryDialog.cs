using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HistoryDialog : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private readonly List<FXText.TMProEffect> textEffects = new List<FXText.TMProEffect>();

    public void SetData(DialogHistory.HistoryData data)
    {
        Cleanup();        
        // Set text
        text.text = DialogParser.instance.Parse(data.ToString(), gameObject, text, textEffects, out _);
    }

    public void Cleanup()
    {
        // Remove old text effects.
        foreach(var effect in textEffects)
        {
            Destroy(effect);
        }
        textEffects.Clear();
        text.text = string.Empty;
    }
}
