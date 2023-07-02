using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuSlider : Selectable
{
    private static readonly char[] trim = new char[] { '>', ' ', ' ', '<', '-' };
    [SerializeField] private AudioClip selectSFX;
    [SerializeField] private AudioClip adjustSfx;
    [SerializeField] private AudioClip adjustFailedSfx;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image selectorImage;
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite deselectedSprite;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI numberText;
    private bool selected = false;
    public bool SkipSelectSfx { get; set; }
    public void InitializeSelection()
    {
        SkipSelectSfx = true;
        Select();
    }

    private void Update()
    {
        if (!selected)
            return;
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                AdjustValue(1);
            }
            else
            {
                AdjustValue(5);
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                AdjustValue(-1);
            }
            else
            {
                AdjustValue(-5);
            }
        }
    }

    private void AdjustValue(int delta)
    {
        if(delta >= 0)
        {
            if (slider.value >= slider.maxValue)
            {
                AudioManager.instance.PlaySFX(adjustFailedSfx);
            }
            else
            {
                AudioManager.instance.PlaySFX(adjustSfx);
            }
        }
        else if (slider.value <= slider.minValue)
        {
            AudioManager.instance.PlaySFX(adjustFailedSfx);
        }
        else
        {
            AudioManager.instance.PlaySFX(adjustSfx);
        }
        SetValue((int)slider.value + delta);
    }

    private void SetValue(int value)
    {
        slider.value = value;
        numberText.text = slider.value.ToString();
    }

    public override void OnSelect(BaseEventData eventData)
    {
        //Do this on highlight
        text.color = MenuButton.selectedColor;
        text.text = $"<- {text.text} ->";
        selectorImage.sprite = selectedSprite;
        if (SkipSelectSfx)
        {
            SkipSelectSfx = false;
        }
        else
        {
            AudioManager.instance.PlaySFX(selectSFX);
        }
        selected = true;
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        //Do this on un-highlight
        text.color = Color.white;
        text.text = text.text.Trim(trim);
        selectorImage.sprite = deselectedSprite;
        selected = false;
    }

    public void OnSubmit(BaseEventData eventData)
    {
        AudioManager.instance.PlaySFX(adjustSfx);
    }
}
