using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HistoryDialog : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public void SetData(DialogHistory.HistoryData data)
    {
        text.text = $"{data.Speaker}: {data.Text}";
    }
}
