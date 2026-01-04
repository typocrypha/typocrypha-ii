using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ControlGuide : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] controlTexts;

    public void SetControl(int index, string text)
    {
        controlTexts[index].text = text;
    }
}
