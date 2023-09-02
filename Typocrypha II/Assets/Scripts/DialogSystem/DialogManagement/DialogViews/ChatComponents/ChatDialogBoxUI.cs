using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatDialogBoxUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;

    public void Bind(DialogItemChat dialogItem)
    {
        timeText.text = dialogItem.timeText;
    }
}
