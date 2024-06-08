using UnityEngine;
using TMPro;

public class LabeledContent : MonoBehaviour
{
    public TextMeshProUGUI Label;
    public TextMeshProUGUI Content;

    public void SetLabel(string text)
    {
        Label.text = text;
    }

    public void SetContent(string text)
    {
        Content.text = text;
    }

    public void SetLabelAndContent(string labelText, string contentText)
    {
        Label.text = labelText;
        Content.text = contentText;
    }
}
