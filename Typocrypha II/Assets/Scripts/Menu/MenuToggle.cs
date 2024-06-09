using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class MenuToggle : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler
{
    private static readonly char[] trim = new char[] { '>', ' ', ' ', '<', '-' };
    public static readonly Color selectedColor = new Color(219f / 255f, 56f / 255f, 202f / 255f);
    [SerializeField] private AudioClip selectSFX;
    [SerializeField] private AudioClip enterSFX;
    [SerializeField] private TextMeshProUGUI selectorText;
    [SerializeField] private TextMeshProUGUI numberText;
    [SerializeField] private Button button;
    [SerializeField] private Image selectorImage;
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite deselectedSprite;
    private bool selected = false;
    private bool toggleValue = false;
    public bool SkipSelectSfx { get; set; }
    public void InitializeSelection()
    {
        SkipSelectSfx = true;
        button.Select();
    }

    public void SetValue(bool value)
    {
        toggleValue = value;
        numberText.text = value ? "ON" : "OFF";
    }

    public void OnSelect(BaseEventData eventData)
    {
        //Do this on highlight
        selectorText.color = selectedColor;
        selectorText.text = $"> {selectorText.text} <";
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

    public void OnDeselect(BaseEventData eventData)
    {
        //Do this on un-highlight
        selectorText.color = Color.white;
        selectorText.text = selectorText.text.Trim(trim);
        selectorImage.sprite = deselectedSprite;
        selected = false;
    }

    public void OnSubmit(BaseEventData eventData)
    {
        AudioManager.instance.PlaySFX(enterSFX);
        SetValue(!toggleValue);
    }
}
