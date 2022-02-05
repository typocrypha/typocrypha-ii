using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class DialogViewVNPlus : DialogView
{
    private const float tweenTime = 0.5f;

    public enum CharacterColumn
    {
        Left,
        Right
    }

    [SerializeField] private GameObject rightDialogBoxPrefab;
    [SerializeField] private GameObject leftDialogBoxPrefab;
    [SerializeField] private RectTransform messageContainer;
    [SerializeField] private VerticalLayoutGroup messageLayout;
    [SerializeField] private Ease messageLayoutEase;
    [SerializeField] private bool useCustomMessageLayoutEase;
    [SerializeField] private AnimationCurve customMessageLayoutEase;
    [SerializeField] private VNPlusCharacter speakingCharacter;
    [SerializeField] private GameObject characterPrefab;

    public override bool ReadyToContinue => readyToContinue;

    private bool readyToContinue = false;
    private Tween tween;

    private readonly Dictionary<string, VNPlusCharacter> characterMap = new Dictionary<string, VNPlusCharacter>(4);

    public override void CleanUp()
    {

    }

    public void AddCharacter(CharacterData data, CharacterColumn column)
    {
        // Main character
        if (data.IsNamed(PlayerDataManager.instance.Get<string>(PlayerDataManager.mainCharacterName)))
        {
            speakingCharacter.Data = data;
        }
        else if(!characterMap.ContainsKey(data.name))// Scene character
        {
            var newCharacter = Instantiate(characterPrefab).GetComponent<VNPlusCharacter>(); // need to be put in a specific transform
            newCharacter.Data = data;
            characterMap.Add(data.name, newCharacter);
        }
    }

    public void SetExpression(CharacterData data, string expression)
    {
        if (data.IsNamed(PlayerDataManager.instance.Get<string>(PlayerDataManager.mainCharacterName)))
        {
            speakingCharacter.SetExpression(expression);
        }
        else if (characterMap.ContainsKey(data.name))// Scene character
        {
            characterMap[data.name].SetExpression(expression);
        }
    }

    public void SetPose(CharacterData data, string pose)
    {
        if (data.IsNamed(PlayerDataManager.instance.Get<string>(PlayerDataManager.mainCharacterName)))
        {
            speakingCharacter.SetPose(pose);
        }
        else if (characterMap.ContainsKey(data.name))// Scene character
        {
            characterMap[data.name].SetPose(pose);
        }
    }

    public override DialogBox PlayDialog(DialogItem data)
    {
        if (!IsDialogItemCorrectType(data, out DialogItemVNPlus dialogItem))
            return null;
        var prefab = dialogItem.IsLeft ? leftDialogBoxPrefab : rightDialogBoxPrefab;
        var dialogBox = Instantiate(prefab, messageContainer).GetComponent<DialogBox>();
        readyToContinue = false;
        StartCoroutine(AnimateNewMessageIn(dialogBox, dialogItem));
        return dialogBox;
    }

    private IEnumerator AnimateNewMessageIn(DialogBox box, DialogItem item)
    {
        box.SetupDialogBox(item);
        yield return null;
        box.SetBoxHeight();
        tween?.Complete();
        tween = messageContainer.DOAnchorPosY(messageContainer.anchoredPosition.y + (box.GetBoxHeight() + messageLayout.spacing), tweenTime);
        // Play animation
        if (useCustomMessageLayoutEase)
        {
            tween.SetEase(customMessageLayoutEase);
        }
        else
        {
            tween.SetEase(messageLayoutEase);
        }
        readyToContinue = true;
        box.StartDialogScroll();
        yield break;
    }

    public override void SetEnabled(bool e)
    {
        gameObject.SetActive(e);
    }
}
