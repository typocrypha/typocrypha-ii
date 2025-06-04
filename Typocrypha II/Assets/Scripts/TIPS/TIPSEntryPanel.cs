using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TIPSEntryPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI content;

    public void SetTitle(string text)
    {
        title.text = text;
    }
    public void SetContent(string text)
    {
        content.text = text;
    }
}
