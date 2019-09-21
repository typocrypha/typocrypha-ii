using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleLog : MonoBehaviour
{
    public Text text;
    public Image icon;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetContent(string text, Sprite icon)
    {
        this.text.text = text;
        this.icon.sprite = icon;
    }
}
