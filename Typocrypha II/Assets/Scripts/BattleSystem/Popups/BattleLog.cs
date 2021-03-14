using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleLog : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Image icon;

    public void SetContent(string text, Sprite icon)
    {
        this.text.text = text;
        this.icon.sprite = icon;
        this.icon.color = icon == null ? Color.clear : Color.white;
    }
}
