using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

// A dialog view with a message box like VN+ or Chat
public abstract class DialogViewMessage<T> : DialogView where T : DialogItemMessage
{
    private const int maxMessages = 7;
    private const int messagePrefabTypes = 3;

    [SerializeField] protected RectTransform messageContainer;
    [SerializeField] private VerticalLayoutGroup messageLayout;
    [SerializeField] private TweenInfo messageTween;
    [SerializeField] private TweenInfo messageScaleTween;
    [SerializeField] private TweenInfo messageFadeTween;
    [SerializeField] private TextMeshProUGUI locationText;
    [SerializeField] private TextMeshProUGUI dateTimeText;

    private readonly List<DialogBox> activeDialogBoxes = new List<DialogBox>(maxMessages * (messagePrefabTypes + 1));
    private readonly List<DialogBox> dialogBoxPoolHidden = new List<DialogBox>(maxMessages * (messagePrefabTypes + 1));
    private VNPlusDialogBoxUI lastBoxUI = null;
    private float originalMessageAnchorPosY = float.MinValue;

    public override bool ShowImmediately => false;

    private void Awake()
    {
        originalMessageAnchorPosY = messageContainer.anchoredPosition.y;
    }

    protected DialogBox CreateNewMessage(T dialogItem)
    {
        var prefab = GetMessagePrefab(dialogItem, dialogItem.CharacterData, out bool isNarrator);
        var dialogBox = CreateDialogBox(prefab);
        dialogBox.transform.SetAsFirstSibling();
        var dialogBoxUI = dialogBox.GetComponent<VNPlusDialogBoxUI>();
        SetCharacterSpecificUI(dialogBoxUI, dialogItem.CharacterData);
        AnimateNewMessageIn(dialogBox, dialogBoxUI, dialogItem, isNarrator);
        return dialogBox;
    }

    public void CreateEmbeddedImage(Sprite sprite, int messagesBeforeFade)
    {
        var instance =  Instantiate(GetImagePrefab(), messageContainer);
        var image = instance.GetComponentInChildren<Image>();
        image.sprite = sprite;
        var imageAspect = (float)image.sprite.texture.width / image.sprite.texture.height;
        var maxSize = image.rectTransform.sizeDelta;
        image.rectTransform.sizeDelta = new Vector2(
            Mathf.Min(maxSize.y * imageAspect, maxSize.x),
            Mathf.Min(maxSize.x / imageAspect, maxSize.y));
        instance.transform.SetAsFirstSibling();
        instance.GetComponent<VNPlusEmbeddedImage>().messagesBeforeFade = messagesBeforeFade;
        instance.GetComponentInChildren<DialogContinueIndicator>().Activate();
        LayoutRebuilder.ForceRebuildLayoutImmediate(instance.transform as RectTransform);
        AnimateNewImageIn(instance.transform as RectTransform);
    }

    private void SetCharacterSpecificUI(VNPlusDialogBoxUI vnPlusUI, List<CharacterData> data)
    {
        if (vnPlusUI == null)
            return;
        if (data.Count > 1)
        {
            throw new System.NotImplementedException("Message-based modes don't currently support multi-character dialog lines");
        }
        if (data.Count <= 0)
        {
            throw new System.NotImplementedException("Message-based modes don't currently support system dialog lines");
        }
        vnPlusUI.Bind(data[0]);
    }

    protected abstract GameObject GetMessagePrefab(T dialogItem, List<CharacterData> data, out bool isNarrator);
    protected abstract GameObject GetImagePrefab();

    private DialogBox CreateDialogBox(GameObject prefab)
    {
        // Try Reusing active messages that are past the maximum
        if (activeDialogBoxes.Count > maxMessages)
        {
            var prefabId = prefab.GetComponent<DialogBox>().ID;
            for (int i = 0; i < activeDialogBoxes.Count - maxMessages; ++i)
            {
                if (prefabId == activeDialogBoxes[i].ID)
                {
                    var dialogBox = activeDialogBoxes[i];
                    activeDialogBoxes.RemoveAt(i);
                    activeDialogBoxes.Add(dialogBox);
                    return dialogBox;
                }
            }
        }
        // Try using an offscreen dialog box from the pool
        if (dialogBoxPoolHidden.Count > 0)
        {
            var prefabId = prefab.GetComponent<DialogBox>().ID;
            for (int i = 0; i < dialogBoxPoolHidden.Count; ++i)
            {
                if (prefabId == dialogBoxPoolHidden[i].ID)
                {
                    var dialogBox = dialogBoxPoolHidden[i];
                    dialogBoxPoolHidden.RemoveAt(i);
                    activeDialogBoxes.Add(dialogBox);
                    dialogBox.CanvasGroup.alpha = 1;
                    return dialogBox;
                }
            }
        }
        // Else, create a new dialog box
        var newDialogBox = Instantiate(prefab, messageContainer).GetComponent<DialogBox>();
        activeDialogBoxes.Add(newDialogBox);
        return newDialogBox;
    }

    private void AnimateNewMessageIn(DialogBox box, VNPlusDialogBoxUI vNPlusDialogUI, T item, bool isNarrator)
    {
        box.SetupDialogBox(item);
        CompleteMessageTweens();
        messageLayout.CalculateLayoutInputVertical();
        var yTemp = messageContainer.anchoredPosition.y;
        messageContainer.anchoredPosition = new Vector2(messageContainer.anchoredPosition.x, messageContainer.anchoredPosition.y + (box.GetBoxHeight() + messageLayout.spacing));
        messageTween.Start(messageContainer.DOAnchorPosY(yTemp, messageTween.Time));
        // Initialize scale
        if (isNarrator)
        {
            box.transform.localScale = new Vector3(1, 1, box.transform.localScale.z);
        }
        else
        {
            box.transform.localScale = new Vector3(0, 0, box.transform.localScale.z);
            messageScaleTween.Start(box.transform.DOScale(new Vector3(1, 1, box.transform.localScale.z), messageScaleTween.Time));
        }
        if (lastBoxUI != null)
        {
            lastBoxUI.DoDim(messageFadeTween);
        }
        lastBoxUI = vNPlusDialogUI;
        box.StartDialogScroll();
    }
    //TODO: Refactor with AnimateNewMessageIn
    protected void AnimateNewImageIn(RectTransform imageTransform)
    {
        CompleteMessageTweens();
        messageLayout.CalculateLayoutInputVertical();
        var yTemp = messageContainer.anchoredPosition.y;
        messageContainer.anchoredPosition = new Vector2(messageContainer.anchoredPosition.x, messageContainer.anchoredPosition.y + (imageTransform.sizeDelta.y + messageLayout.spacing));
        messageTween.Start(messageContainer.DOAnchorPosY(yTemp, messageTween.Time));
        // Initialize scale
        imageTransform.localScale = new Vector3(0, 0, imageTransform.localScale.z);
        messageScaleTween.Start(imageTransform.DOScale(new Vector3(1, 1, imageTransform.localScale.z), messageScaleTween.Time));
    }

    protected void CompleteMessageTweens()
    {
        messageTween.Complete();
        messageScaleTween.Complete();
        messageFadeTween.Complete();
    }

    public override void CleanUp()
    {
        StopAllCoroutines();
        ClearLog();
    }

    public override Coroutine Clear()
    {
        ClearLog();
        return null;
    }

    private void ClearLog()
    {
        StopAllCoroutines();
        CompleteMessageTweens();
        foreach (var box in activeDialogBoxes)
        {
            box.CanvasGroup.alpha = 0;
            dialogBoxPoolHidden.Add(box);
        }
        activeDialogBoxes.Clear();
        if (originalMessageAnchorPosY != float.MinValue)
        {
            messageContainer.anchoredPosition = new Vector2(messageContainer.anchoredPosition.x, originalMessageAnchorPosY);
        }
    }

    protected override void SetLocation(string location)
    {
        base.SetLocation(location);
        locationText.text = location;
    }

    protected override void SetDateTime(string dateTime)
    {
        base.SetDateTime(dateTime);
        dateTimeText.text = dateTime;
    }
}
