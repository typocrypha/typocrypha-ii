using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum IconSide { LEFT, RIGHT, BOTH, NONE }; // Side which icon displays

/// <summary>
/// Chat style dialog. i.e. chatboxes on scrollable feed.
/// </summary>
public class DialogViewChat : DialogView
{
    public RectTransform ChatContent; // Content of chat scroll view (contains dialogue boxes)
    public Scrollbar scrollBar; // Scroll bar of chat dialogue window
    public float defaultWindowHeight = 16f; // Default chat window height
    public float scrollBarTime = 0.15f; // Time it takes to automatically update window

    float windowHeight; // Height of chat content window
    Coroutine slideScrollCR; // Coroutine for smoothly sliding scroll bar
    
    void Awake()
    {
        windowHeight = defaultWindowHeight;
    }

    public override DialogBox PlayDialog(DialogItem data)
    {
        #region Check Arguments
        DialogItemChat item = data as DialogItemChat;
        if (item == null)
        {
            throw new System.Exception("Incorrect Type of dialog Item for the Chat " +
                                       "view mode (requires DialogItemChat)");
        }
        #endregion

        #region Instantiate and initialize new Dialog boxDialogueRightIcon
        GameObject obj = GameObject.Instantiate(dialogBoxPrefab, ChatContent);
        Image leftIcon = obj.transform.Find("DialogueLeftIcon").GetComponent<Image>();
        Image rightIcon = obj.transform.Find("DialogueRightIcon").GetComponent<Image>();
        if (item.iconSide == IconSide.LEFT || item.iconSide == IconSide.BOTH)
        {
            leftIcon.sprite = item.leftIcon;
            leftIcon.enabled = true;
        }
        if (item.iconSide == IconSide.RIGHT || item.iconSide == IconSide.BOTH)
        {
            rightIcon.sprite = item.rightIcon;
            rightIcon.enabled = true;
        }
        DialogBox dialogBox = obj.GetComponent<DialogBox>();
        #endregion

        dialogBox.StartDialogBox(item);
        SetWindowSize(dialogBox.GetBoxHeight() + ChatContent.GetComponent<VerticalLayoutGroup>().spacing);
        return dialogBox;
    }

    public override void SetEnabled(bool e)
    {
        gameObject.SetActive(e);
        if (!e)
        {
            ClearLog();
        }
    }

    // Remove all chat messages
    public void ClearLog()
    {
        foreach (Transform child in ChatContent)
        {
            Destroy(child.gameObject);
        }
        ResetWindowSize();
    }

    public override void CleanUp()
    {
        ClearLog();
    }

    #region chat window height management
    // Increases window size to fit new dialogue box
    void SetWindowSize(float boxHeight)
    {
        windowHeight += boxHeight;
        ChatContent.sizeDelta = new Vector2(ChatContent.sizeDelta.x, windowHeight);
        StopSlideScroll();
        slideScrollCR = StartCoroutine(SlideScroll());
    }
    
    // Resets height of chat window
    void ResetWindowSize()
    {
        windowHeight = defaultWindowHeight;
        ChatContent.sizeDelta = new Vector2(ChatContent.sizeDelta.x, windowHeight);
        StopSlideScroll();
    }

    // Stops slide adjustment of window
    void StopSlideScroll()
    {
        if (slideScrollCR != null) StopCoroutine(slideScrollCR);
    }

    // Coroutine that smoothly slides scroll bar to bottom
    IEnumerator SlideScroll()
    {
        yield return new WaitUntil(() => scrollBar.value > Mathf.Epsilon);
        float vel = 0;
        while (scrollBar.value > Mathf.Epsilon)
        {
            scrollBar.value = Mathf.SmoothDamp(scrollBar.value, 0, ref vel, scrollBarTime);
            yield return null;
        }
    }
    #endregion
}
