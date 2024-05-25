//using System.Collections;
//using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler
{
    public UnityEvent onSelect = default;

    private static readonly char[] trim = new char[] { '>', ' ', ' ', '<' };
    public static readonly Color selectedColor = new Color(219f / 255f, 56f / 255f, 202f / 255f);
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

    public void Select()
    {
        button.Select();
    }

    public void OnSelect(BaseEventData eventData)
    {
        //Do this on highlight
        text.color = selectedColor;
        TrimText();
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
        onSelect.Invoke();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        //Do this on un-highlight
        text.color = Color.white;
        TrimText();
        buttonImage.sprite = deselectedSprite;
    }

    public void TrimText()
    {
        text.text = text.text.Trim(trim);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        AudioManager.instance.PlaySFX(enterSFX);
    }

    public void SetText(string newText)
    {
        text.text = newText;
    }
}
