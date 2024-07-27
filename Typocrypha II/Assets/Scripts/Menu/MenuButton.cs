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
    public Color selectedColor = new Color(219f / 255f, 56f / 255f, 202f / 255f);
    public Color defaultColor = Color.white;
    [SerializeField] private AudioClip selectSFX;
    [SerializeField] private AudioClip enterSFX;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] public Button button;
    [SerializeField] private Image buttonImage;
    [SerializeField] private string selectionPrefix = "> ";
    [SerializeField] private string selectionSuffix = " <";
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite deselectedSprite;
    [SerializeField] private KeyCode[] shortcutKeys = default;
    public bool SkipSelectSfxOnce { get; set; }

    private EventSystem currentES;

    private void OnEnable()
    {
        currentES = EventSystem.current;
        ToggleHighlight(false);
    }

    private void Update()
    {
        if (shortcutKeys.Length == 0) return;
        //check selection for context
        if (currentES == null) return;
        if (currentES.currentSelectedGameObject == null) return;
        if (currentES.currentSelectedGameObject.transform.parent != transform.parent) return;

        foreach (var key in shortcutKeys)
        {
            if (Input.GetKeyDown(key))
            {
                ExecuteEvents.Execute(button.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
                return;
            }
        }
    }

    public void InitializeSelection()
    {
        SkipSelectSfxOnce = true;
        button.Select();
    }

    public void Select()
    {
        button.Select();
    }

    public void OnSelect(BaseEventData eventData)
    {
        ToggleHighlight(true);
        if (SkipSelectSfxOnce)
        {
            SkipSelectSfxOnce = false;
        }
        else
        {
            if (selectSFX) AudioManager.instance.PlaySFX(selectSFX);
        }
        onSelect.Invoke();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        ToggleHighlight(false);
    }

    public void ToggleHighlight(bool value)
    {
        if (value)
        {
            //Do this on highlight
            text.color = selectedColor;
            TrimText();
            text.text = selectionPrefix + text.text + selectionSuffix;
            buttonImage.sprite = selectedSprite;
        }
        else
        {
            //Do this on un-highlight
            text.color = defaultColor;
            TrimText();
            buttonImage.sprite = deselectedSprite;
        }
    }

    public void TrimText()
    {
        text.text = text.text.Trim(trim);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        if (enterSFX) AudioManager.instance.PlaySFX(enterSFX);
    }

    public void SetText(string newText)
    {
        text.text = newText;
    }
}
