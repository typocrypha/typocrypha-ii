using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler
{
    private static readonly char[] trim = new char[] { '>', ' ', ' ', '<' };
    [SerializeField] private AudioClip selectSFX;
    [SerializeField] private AudioClip enterSFX;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Button button;
    [SerializeField] private Image buttonImage;
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite deselectedSprite;
    public bool SkipSelectSfx { get; set; }
    public void InitializeSelection()
    {
        SkipSelectSfx = true;
        button.Select();
    }

    public void OnSelect(BaseEventData eventData)
    {
        //Do this on highlight
        text.color = new Color(219f / 255f, 56f / 255f, 202f / 255f); //magenta
        text.text = "> " + text.text + " <";
        buttonImage.sprite = selectedSprite;
        if (SkipSelectSfx)
        {
            SkipSelectSfx = false;
        }
        else
        {
            AudioManager.instance.PlaySFX(selectSFX);
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        //Do this on un-highlight
        text.color = Color.white;
        text.text = text.text.Trim(trim);
        buttonImage.sprite = deselectedSprite;
    }

    public void OnSubmit(BaseEventData eventData)
    {
        AudioManager.instance.PlaySFX(enterSFX);
    }
}
