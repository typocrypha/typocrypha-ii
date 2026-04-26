using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Audio novel style dialog. i.e. Floating text over full background.
/// </summary>
public class DialogViewAN : DialogView
{
    private const int maxMessages = 10;
    [SerializeField] private RectTransform ANContent; // Content of scroll view
    [SerializeField] private VerticalLayoutGroup ANLayout;
    [SerializeField] private Image background;
    [SerializeField] private DialogContinueIndicator continueIndicator;

    public override bool DeactivateOnEndSceneHide => false;

    private PrefabPool<DialogBox> dialogBoxPool;
    private readonly List<DialogBox> activeDialogBoxes = new List<DialogBox>(maxMessages);
    private readonly Queue<DialogBox> queuedDialogBoxes = new Queue<DialogBox>(maxMessages);

    private void Awake()
    {
        background.color = Color.clear;
        dialogBoxPool = new PrefabPool<DialogBox>(dialogBoxPrefab, ANContent, maxMessages);
    }

    public override DialogBox PlayDialog(DialogItem data)
    {
        if (data is DialogItemQueued)
        {
            if(queuedDialogBoxes.Count <= 0)
            {
                Debug.LogError("Tried to dequeue AN dialog box with empty queue");
                return null;
            }
            var queuedDialogBox = queuedDialogBoxes.Dequeue();
            queuedDialogBox.StartDialogScroll();
            return queuedDialogBox;
        }
        if (!IsDialogItemCorrectType(data, out DialogItemAN dialogItem))
            return null;
        if(activeDialogBoxes.Count + dialogItem.QueuedItems.Count >= maxMessages)
        {
            ClearLog();
        }
        if (ANLayout.childAlignment != dialogItem.LayoutSetting)
        {
            ANLayout.childAlignment = dialogItem.LayoutSetting;
        }
        var dialogBox = CreateDialogBox(dialogItem);
        foreach(var item in dialogItem.QueuedItems)
        {
            queuedDialogBoxes.Enqueue(CreateDialogBox(item));
        }
        dialogBox.StartDialogScroll();
        return dialogBox;
    }

    private DialogBox CreateDialogBox(DialogItemAN dialogItem)
    {
        var dialogBox = dialogBoxPool.Get();
        dialogBox.transform.SetAsLastSibling();
        activeDialogBoxes.Add(dialogBox);
        dialogBox.ContinueIndicator = continueIndicator;
        dialogBox.DialogText.alignment = dialogItem.AlignmentOptions;
        dialogBox.SetupDialogBox(dialogItem);
        return dialogBox;
    }

    public override void SetEnabled(bool e)
    {
        gameObject.SetActive(e);
        if (!e)
        {
            ClearLog();
            background.color = Color.clear;
        }
    }

    public Coroutine FadeBG(float target, float time, System.Action onComplete = null)
    {
        return StartCoroutine(FadeBGCR(target, time, onComplete));
    }

    private IEnumerator FadeBGCR(float target, float time, System.Action onComplete)
    {
        var fadeIn = background.DOFade(target, time);
        yield return fadeIn.WaitForCompletion();
        onComplete?.Invoke();
    }

    public override IEnumerator PlayEnterAnimation(bool firstView)
    {
        if (firstView)
        {
            background.color = new Color(background.color.r, background.color.g, background.color.b, 1);
            yield return new WaitForSeconds(LoadingScreenDefault.fadeTimeStaggered); // wait for loading screen
        }
        else
        {
            var fadeIn = background.DOFade(1, 2);
            yield return fadeIn.WaitForCompletion();
        }
    }

    public override IEnumerator PlayExitAnimation(DialogManager.EndType endType)
    {
        if (endType == DialogManager.EndType.SceneEnd)
            yield break;
        int count = activeDialogBoxes.Count;
        var clear = Clear(false);
        if (clear != null)
        {
            yield return new WaitForSeconds((DialogBox.fadeTime + (count * clearStagger)) * 0.66f);
        }
        var fadeOut = background.DOFade(0, 2);
        yield return fadeOut.WaitForCompletion();
    }

    public override Coroutine Clear(bool instant)
    {
        if (activeDialogBoxes.Count <= 0)
            return null;
        if (instant)
        {
            ClearLog();
            return null;
        }
        return StartCoroutine(ClearCR());
    }

    private const float clearStagger = 0.1f;
    private IEnumerator ClearCR()
    {
        Coroutine fadeCr = null;
        for (int i = 0; i < activeDialogBoxes.Count; i++)
        {
            var dialogBox = activeDialogBoxes[i];
            fadeCr = StartCoroutine(dialogBox.FadeText());
            yield return new WaitForSeconds(clearStagger);
        }
        yield return fadeCr;
        ClearLog();
    }

    private void ClearLog()
    {
        dialogBoxPool.Release(activeDialogBoxes);
        activeDialogBoxes.Clear();
    }

    public override void CleanUp()
    {
        ClearLog();
    }
}
